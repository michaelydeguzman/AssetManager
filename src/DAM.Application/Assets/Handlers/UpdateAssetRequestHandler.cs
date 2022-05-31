using AutoMapper;
using Azure.Storage.Blobs.Models;
using DAM.Application.Assets.Dtos;
using DAM.Application.Assets.Enums;
using DAM.Application.Assets.Requests;
using DAM.Application.AuditTrail.Dtos;
using DAM.Application.AuditTrail.Enums;

using DAM.Application.Cache;
using DAM.Application.Extensions;
using DAM.Application.Services.Interfaces;
using DAM.Domain.Entities;
using DAM.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;

namespace DAM.Application.Assets.Handlers
{
    public class UpdateAssetRequestHandler : HandlerBase<UpdateAssetRequest, HandlerResult<UpdateAssetDto>>
    {
        private readonly ICacheProvider _cacheProvider;
        private readonly IDbContext _dbcontext;
        private readonly IMapper _mapper;
        private readonly IConfiguration _configuration;
        private readonly IAzureStorageService _azureStorageService;
        private readonly IConversionService _conversionService;
        private readonly IHelperService _helperService;

        public UpdateAssetRequestHandler(ICacheProvider cacheProvider, IDbContext dbcontext, IMapper mapper, IConfiguration configuration, IAzureStorageService azureStorageService,
            IConversionService conversionService, IHelperService helperService)
        {
            _cacheProvider = cacheProvider ?? throw new ArgumentNullException(nameof(cacheProvider));
            _dbcontext = dbcontext ?? throw new ArgumentNullException(nameof(dbcontext));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
            _azureStorageService = azureStorageService ?? throw new ArgumentNullException(nameof(azureStorageService));
            _conversionService = conversionService ?? throw new ArgumentNullException(nameof(conversionService));
            _helperService = helperService ?? throw new ArgumentNullException(nameof(helperService));
        }

        public override HandlerResult<UpdateAssetDto> HandleRequest(UpdateAssetRequest request, CancellationToken cancellationToken, HandlerResult<UpdateAssetDto> result)
        {
            var asset = _dbcontext.Assets.Include(a => a.Folder).First(a => a.Id == request.AssetDto.Id);
            var modifiedBy = request.UserId;

            if (asset != null)
            {
                var assetId = (int)request.AssetDto.Id;

                // Retrieve existing details for audit transaction
                var oldAssetAccounts = _dbcontext.AssetAccounts.Where(x => x.AssetId == asset.Id).Include(aa => aa.Account);
                var oldAssetRegions = _dbcontext.AssetCountryRegions.Where(x => x.AssetId == asset.Id).Include(acr => acr.Region).Include(r => r.Country);

                var activeVersion = _dbcontext.AssetVersions.FirstOrDefault(a => a.AssetId == asset.Id && a.ActiveVersion == 1);
                var oldTags = _dbcontext.Tags.Where(t => t.AssetId == activeVersion.Id);
              

                var oldAccounts = oldAssetAccounts.Select(a => a.Account.Description).ToList();
                var oldRegions = oldAssetRegions.Select(ar => ar.Region.Description).ToList();
                var oldCountries = oldAssetRegions.Select(ar => ar.Country.Name).Distinct().ToList();
                var oldTagsList = oldTags.Select(t => t.Name).ToList();

                var oldAssetAuditDetails = new AssetAuditDetailDto()
                {
                    DisplayName = asset.Title,
                    Description = asset.Description,
                    Accounts = oldAccounts,
                    Countries = oldCountries,
                    Regions = oldRegions,
                    Folder = asset.Folder.FolderName,
                    Tags = oldTagsList,
                    Expiry = asset.ExpiryDate,
                    ShareFolders = String.IsNullOrEmpty(asset.ShareFolderIds) ? "" : asset.ShareFolderIds,
                    Status = asset.Status.ToString()
                };

                asset.Title = request.AssetDto.Name;
                asset.ModifiedDate = DateTimeOffset.UtcNow;
                // If createdby is null, default user
                if (string.IsNullOrEmpty(modifiedBy))
                {
                    asset.ModifiedById = _dbcontext.AppUsers.First(u => u.Email == _configuration["DefaultUser"]).Id;
                }
                else
                {
                    asset.ModifiedById = modifiedBy;
                }
                asset.ExpiryDate = request.AssetDto.ExpiryDate;

                var comments = request.AssetDto.Description;
                if (comments != null)
                {
                    asset.Description = comments;
                }

                if (request.AssetDto.FileBytes.Length > 0)
                {
                    // Update File Thumbnail from Storage
                    using (Stream stream = new MemoryStream(request.AssetDto.FileBytes))
                    {
                        if (request.AssetDto.FileType.Contains("image") || request.AssetDto.FileType.Contains("pdf"))
                        {
                            Stream thumbnailStream = stream;
                            BlobProperties ap = new BlobProperties();
                            if (request.AssetDto.FileType.Contains("pdf"))
                            {
                                thumbnailStream = _conversionService.GetPDFThumbnail(_configuration, thumbnailStream);
                            }

                            thumbnailStream = _conversionService.GetImageThumbnail(_configuration, thumbnailStream, request.AssetDto.Extension);

                            BlobProperties thumbnailProps = _azureStorageService.UpdateFileFromStorage(_configuration, thumbnailStream, string.Concat(activeVersion.Key, ".", "Jpeg"),
                                                            out Uri thumbnailUri, true);
                            activeVersion.Thumbnail = _azureStorageService.GetBlobSasUri(_configuration, string.Concat(activeVersion.Key, ".", "Jpeg"), string.Empty, string.Empty, true);
                            ap = _azureStorageService.UploadFileToStorage(_configuration, stream, string.Concat(activeVersion.Key, ".", request.AssetDto.Extension), out Uri assetUri);
                            activeVersion.OriginalUrl = _azureStorageService.GetBlobSasUri(_configuration, string.Concat(activeVersion.Key, ".", request.AssetDto.Extension), string.Empty, request.AssetDto.FileType, false);
                            activeVersion.DownloadUrl = _azureStorageService.GetBlobSasUri(_configuration, string.Concat(activeVersion.Key, ".", request.AssetDto.Extension), activeVersion.FileName, string.Empty, false, true);
                        }
                    }
                    _dbcontext.AssetVersions.Update(activeVersion);
                }

                if (request.AssetDto.ShareFolderIds != null)
                {
                    var shareIds = "";
                    var shareFolderIds = new List<string>();
                    request.AssetDto.ShareFolderIds.Split(',').ToList().ForEach((id) => { if (id != asset.FolderId.ToString()) { shareFolderIds.Add(id); } });
                    shareIds = string.Join(",", shareFolderIds);
                    asset.ShareFolderIds = shareIds;
                }
                
                _dbcontext.Assets.Update(asset);
                
                var accounts = request.AssetDto.Accounts;
                if (accounts != null)
                {
                    //Override current accounts
                    _dbcontext.AssetAccounts.RemoveRange(oldAssetAccounts);

                    foreach (var account in accounts)
                    {
                        _dbcontext.AssetAccounts.Add(new AssetAccountMetaData()
                        {
                            AssetId = (int)(asset.Id),
                            AccountId = (account.Id)
                        });
                    }
                }

                var regions = request.AssetDto.Regions;
                if (regions != null)
                {
                    //Override current countries and regions
                    _dbcontext.AssetCountryRegions.RemoveRange(oldAssetRegions);

                    foreach (var region in regions)
                    {
                        _dbcontext.AssetCountryRegions.Add(new AssetCountryRegionMetaData()
                        {
                            AssetId = (int)(asset.Id),
                            RegionId = (region.Id),
                            CountryId = (region.CountryId)
                        });
                    }
                }

                var tagsToUpdate = request.AssetDto.Tags;
                if (tagsToUpdate != null)
                {
                    var tagsToAdd = new List<Tag>();
                    foreach (var tag in tagsToUpdate)
                    {
                        if (!oldTags.Any(t => t.Name == tag.Name))
                        {
                            var newTag = new Tag
                            {
                                AssetId = activeVersion.Id,
                                Name = tag.Name,
                                IsCognitive = tag.IsCognitive
                            };
                            tagsToAdd.Add(newTag);
                        }
                    }

                    if (tagsToAdd.Count > 0)
                    {
                        _dbcontext.Tags.AddRange(tagsToAdd);
                    }

                    var tagsToRemove = _dbcontext.Tags.Where(t => t.AssetId == activeVersion.Id && !tagsToUpdate.Select(x => x.Name).Contains(t.Name)).ToList();

                    if (tagsToRemove.Count > 0)
                    {
                        _dbcontext.Tags.RemoveRange(tagsToRemove);
                    }
                }

                // Auto-Draft whole asset
                if (asset.Status == (int)AssetStatus.Rejected)
                {
                    asset.Status = (int)AssetStatus.Draft;
                    asset.StatusUpdatedDate = DateTimeOffset.UtcNow;
                    activeVersion.Status = (int)AssetStatus.Draft;
                    activeVersion.StatusUpdatedDate = DateTimeOffset.UtcNow;
                    _dbcontext.Assets.Update(asset);
                    _dbcontext.AssetVersions.Update(activeVersion);

                    var approvalLevels = _dbcontext.ApprovalLevels.Where(x => x.AssetId == asset.Id).ToList();
                    foreach (ApprovalLevel level in approvalLevels)
                    {
                        level.IsActiveLevel = null;
                        _dbcontext.ApprovalLevels.Update(level);
                    }
                }
                _dbcontext.SaveChanges();

                result.Entity = _mapper.Map<UpdateAssetDto>(asset);

                // Insert into Audit Trail
                var newAccounts = request.AssetDto.Accounts == null ? oldAccounts : request.AssetDto.Accounts.Select(a => a.Name).ToList();
                var newCountries = request.AssetDto.Countries == null ? oldCountries : request.AssetDto.Countries.Select(c => c.Name).ToList();
                var newRegions = request.AssetDto.Regions == null ? oldRegions : request.AssetDto.Regions.Select(r => r.Description).ToList();
                var newTags = request.AssetDto.Tags == null ? oldTagsList : request.AssetDto.Tags.Select(t => t.Name).ToList();

                var newAssetAuditDetails = new AssetAuditDetailDto()
                {
                    DisplayName = asset.Title,
                    Description = asset.Description,
                    Accounts = newAccounts,
                    Countries = newCountries,
                    Regions = newRegions,
                    Folder = asset.Folder.FolderName,
                    Tags = newTags,
                    Expiry = asset.ExpiryDate,
                    ShareFolders = String.IsNullOrEmpty(asset.ShareFolderIds) ? "" : asset.ShareFolderIds,
                    Status = asset.Status.ToString()
                };

                var auditDisplayUser = _dbcontext.AppUsers.First(u => u.Id == request.UserId);

                var auditTrailEntry = new AssetAudit()
                {
                    AssetId = Convert.ToInt32(asset.Id),
                    FolderId = asset.FolderId,
                    AssetFileName = activeVersion.FileName,
                    AuditType = Convert.ToInt32(AssetAuditType.AssetUpdated),
                    AuditTypeText = AssetAuditType.AssetUpdated.GetDescription(),
                    AuditCreatedByUserId = asset.ModifiedById,
                    AuditCreatedDate = (DateTimeOffset)(asset.ModifiedDate),
                    AuditCreatedByName = auditDisplayUser != null ? auditDisplayUser.UserName : "",
                    PreviousParameters = _helperService.GetJsonString(oldAssetAuditDetails),
                    NewParameters = _helperService.GetJsonString(newAssetAuditDetails)
                };

                _dbcontext.AssetAudit.Add(auditTrailEntry);
                _dbcontext.SaveChanges();

            }
            result.ResultType = ResultType.Success;
            return result;
        }

        public override Exception OnException(Exception ex)
        {
            return new Exception(ex.Message, ex);
        }

    }
}