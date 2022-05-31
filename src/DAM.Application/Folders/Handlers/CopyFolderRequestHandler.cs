using AutoMapper;
using DAM.Application.Assets.Enums;
using DAM.Application.AuditTrail.Enums;

using DAM.Application.Cache;
using DAM.Application.Extensions;
using DAM.Application.Folders.Dtos;
using DAM.Application.Services.Interfaces;
using DAM.Domain.Entities;
using DAM.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using static DAM.Application.Folders.Requests.FoldersRequest;

namespace DAM.Application.Folders.Handlers
{
    public class CopyFolderRequestHandler : HandlerBase<CopyFolderRequest, HandlerResult<CopyFolderDto>>
    {
        private readonly ICacheProvider _cacheProvider;
        private readonly IDbContext _dbcontext;
        private readonly IMapper _mapper;
        private readonly IConfiguration _configuration;
        private readonly IAzureStorageService _azureStorageService;

        public CopyFolderRequestHandler(ICacheProvider cacheProvider, IDbContext dbcontext, IMapper mapper, IConfiguration configuration, IAzureStorageService azureStorageService)
        {
            _cacheProvider = cacheProvider ?? throw new ArgumentNullException(nameof(cacheProvider));
            _dbcontext = dbcontext ?? throw new ArgumentNullException(nameof(dbcontext));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
            _azureStorageService = azureStorageService ?? throw new ArgumentNullException(nameof(azureStorageService));
        }

        public override HandlerResult<CopyFolderDto> HandleRequest(CopyFolderRequest request, CancellationToken cancellationToken, HandlerResult<CopyFolderDto> result)
        {
            var newParentFolderId = request.CopyFolderDto.ParentFolderId == 0 ? null : request.CopyFolderDto.ParentFolderId;
            var userId = request.UserId;

            // If user is null, default user
            if (string.IsNullOrEmpty(userId))
            {
                userId = _dbcontext.AppUsers.First(u => u.Email == _configuration["DefaultUser"]).Id;
            }

            var folder = _dbcontext.Folders.First(a => a.Id == request.CopyFolderDto.FolderId);

            if (folder != null)
            {
                // Copy folder
                var newCopyFolder = new Folder()
                {
                    FolderName = folder.FolderName + " (copy)",
                    Description = folder.Description,
                    ParentFolderId = newParentFolderId,
                    OrderNumber = folder.OrderNumber,
                    Deleted = false,
                    CreatedById = userId
                };
                _dbcontext.Folders.Add(newCopyFolder);
                _dbcontext.SaveChanges();

                // Copy folder accounts
                var folderAccounts = _dbcontext.FolderAccounts.Where(f => f.FolderId == folder.Id);

                if (folderAccounts.Count() > 0)
                {
                    foreach (var folderAccount in folderAccounts)
                    {
                        var newFolderAccount = new FolderAccountMetaData()
                        {
                            FolderId = newCopyFolder.Id,
                            AccountId = folderAccount.AccountId,
                            CreatedById = userId
                        };

                        _dbcontext.FolderAccounts.Add(newFolderAccount);
                        _dbcontext.SaveChanges();
                    }
                }

                // Copy folder region country
                var folderCountryRegions = _dbcontext.FolderCountryRegions.Where(f => f.FolderId == folder.Id);

                if (folderCountryRegions.Count() > 0)
                {
                    foreach (var folderCountryRegion in folderCountryRegions)
                    {
                        var newFolderCountryRegion = new FolderCountryRegionMetaData()
                        {
                            FolderId = newCopyFolder.Id,
                            CountryId = folderCountryRegion.CountryId,
                            RegionId = folderCountryRegion.RegionId,
                            CreatedById = userId
                        };

                        _dbcontext.FolderCountryRegions.Add(newFolderCountryRegion);
                        _dbcontext.SaveChanges();
                    }
                }

                // Copy Assets

                var assetsToCopy = _dbcontext.Assets.Where(a => a.FolderId == folder.Id && a.Status != (int)AssetStatus.Archived && a.Status != (int)AssetStatus.Deleted);

                foreach (var asset in assetsToCopy)
                {
                    var assetToCopy = asset;

                    // Copy asset into new asset row

                    var newAsset = new Asset
                    {
                        Title = assetToCopy.Title,
                        FolderId = newCopyFolder.Id,
                        Status = assetToCopy.Status,
                        StatusUpdatedDate = assetToCopy.StatusUpdatedDate,
                        Description = assetToCopy.Description,
                        ExpiryDate = assetToCopy.ExpiryDate,
                        CreatedById = assetToCopy.CreatedById
                    };

                    _dbcontext.Assets.Add(newAsset);
                    _dbcontext.SaveChanges();

                    // Copy Asset versions 

                    var assetVersionsToCopy = _dbcontext.AssetVersions.Where(a => a.AssetId == asset.Id);

                    foreach (var version in assetVersionsToCopy)
                    {
                        var newVersionKey = Guid.NewGuid().ToString().Replace("-", "");
                        var thumbnailExt = string.Empty;
                        bool copyThumbnail = true;

                        // copy blob to new key for asset, thumbnail, downloadUrl

                        _azureStorageService.CopyBlob(_configuration, version.Key + "." + version.Extension, newVersionKey + "." + version.Extension);
                        var newVersionUrl = _azureStorageService.GetBlobSasUri(_configuration, string.Concat(newVersionKey, ".", version.Extension), string.Empty, version.FileType, false);
                        var newDownloadUrl = _azureStorageService.GetBlobSasUri(_configuration, string.Concat(newVersionKey, ".", version.Extension), string.Empty, version.FileType, true);
                        var newThumbnailUrl = "";

                        if (version.FileType.Contains("audio"))
                        {
                            newThumbnailUrl = "audio";
                            copyThumbnail = false;
                        }
                        else if (version.FileType.Contains("otf") || version.FileType.Contains("ttf"))
                        {
                            newThumbnailUrl = "fonts";
                            copyThumbnail = false;
                        }
                        else if (version.FileType.Contains("eps"))
                        {
                            newThumbnailUrl = "eps";
                            copyThumbnail = false;
                        }
                        else if (version.FileType.Contains("zip"))
                        {
                            newThumbnailUrl = "zip";
                            copyThumbnail = false;
                        }
                        else if (version.FileType.Contains("svg"))
                        {
                            newThumbnailUrl = "svg";
                            copyThumbnail = false;
                        }
                        else if (version.FileType.Contains("image"))
                        {
                            if (version.Extension.Contains("tiff") || version.Extension.Contains("tif"))
                            {
                                thumbnailExt = "png";
                            }
                            else
                            {
                                thumbnailExt = "Jpeg";
                            }
                        }
                        else
                        {
                            thumbnailExt = version.Extension;
                        }

                        if (copyThumbnail)
                        {
                            if (version.Thumbnail == version.Extension)
                            {
                                newThumbnailUrl = version.Extension;
                            } 
                            else
                            {
                                _azureStorageService.CopyBlob(_configuration, version.Key + "." + thumbnailExt, newVersionKey + "." + thumbnailExt, true);
                                newThumbnailUrl = _azureStorageService.GetBlobSasUri(_configuration, string.Concat(newVersionKey, ".", thumbnailExt), string.Empty, version.FileType, true);
                            }
                        }

                        var newVersion = new AssetVersions
                        {
                            AssetId = newAsset.Id,
                            FileName = version.FileName,
                            Key = newVersionKey,
                            Extension = version.Extension,
                            Status = version.Status,
                            ActiveVersion = version.ActiveVersion,
                            StatusUpdatedDate = version.StatusUpdatedDate,
                            Description = version.Description,
                            Size = version.Size,
                            FileSizeText = version.FileSizeText,
                            FileType = version.FileType,
                            Version = version.Version,
                            DownloadCount = 0,
                            OriginalUrl = newVersionUrl,
                            Thumbnail = newThumbnailUrl,
                            DownloadUrl = newDownloadUrl,
                            CreatedById = version.CreatedById
                        };

                        _dbcontext.AssetVersions.Add(newVersion);
                        _dbcontext.SaveChanges();

                        // Copy tags per version

                        var tagsToCopy = _dbcontext.Tags.Where(aa => aa.AssetId == version.Id);
                        var newTagsToCreate = new List<Tag>();

                        foreach (var tag in tagsToCopy)
                        {
                            var newTag = new Tag
                            {
                                AssetId = newVersion.Id,
                                IsCognitive = tag.IsCognitive,
                                Name = tag.Name

                            };
                            newTagsToCreate.Add(newTag);
                        }
                        _dbcontext.Tags.AddRange(newTagsToCreate);
                        _dbcontext.SaveChanges();
                    }

                    // Copy asset meta data

                    // accounts
                    var assetAccountsToCopy = _dbcontext.AssetAccounts.Where(aa => aa.AssetId == assetToCopy.Id);

                    if (assetAccountsToCopy.Any())
                    {
                        var accountsToCreate = new List<AssetAccountMetaData>();
                        foreach (var account in assetAccountsToCopy)
                        {
                            var newAssetAccount = new AssetAccountMetaData
                            {
                                AssetId = newAsset.Id,
                                AccountId = account.AccountId,
                                CreatedById = userId
                            };
                            accountsToCreate.Add(newAssetAccount);
                        }
                        _dbcontext.AssetAccounts.AddRange(accountsToCreate);
                        _dbcontext.SaveChanges();
                    }

                    // countries

                    var assetRegionsToCopy = _dbcontext.AssetCountryRegions.Where(aa => aa.AssetId == assetToCopy.Id);

                    if (assetRegionsToCopy.Any())
                    {
                        var assetRegionsToCreate = new List<AssetCountryRegionMetaData>();
                        foreach (var assetRegion in assetRegionsToCopy)
                        {
                            var newAssetRegion = new AssetCountryRegionMetaData
                            {
                                AssetId = newAsset.Id,
                                RegionId = assetRegion.RegionId,
                                CountryId = assetRegion.CountryId,
                                CreatedById = userId
                            };
                            assetRegionsToCreate.Add(newAssetRegion);
                        }
                        _dbcontext.AssetCountryRegions.AddRange(assetRegionsToCreate);
                        _dbcontext.SaveChanges();
                    }
                }


                // Insert into Audit Trail
                var auditDisplayUser = _dbcontext.AppUsers.First(u => u.Id == userId);

                var newFolder = _dbcontext.Folders.First(f => f.Id == newParentFolderId).FolderName;

                var auditTrailEntry = new AssetAudit()
                {
                    FolderId = Convert.ToInt32(folder.Id),
                    FolderName = folder.FolderName,
                    AuditType = Convert.ToInt32(AssetAuditType.FolderCopied),
                    AuditTypeText = AssetAuditType.FolderCopied.GetDescription(),
                    AuditCreatedByUserId = userId,
                    AuditCreatedDate = DateTimeOffset.UtcNow,
                    AuditCreatedByName = auditDisplayUser != null ? auditDisplayUser.UserName : "",
                    PreviousParameters = "",
                    NewParameters = $"Copied to {newFolder} as {newCopyFolder.FolderName}"
                };

                _dbcontext.Add(auditTrailEntry);
                _dbcontext.SaveChanges();
            }

            result.Entity = request.CopyFolderDto;
            result.ResultType = ResultType.Success;

            return result;
        }

        public override Exception OnException(Exception ex)
        {
            return new Exception(ex.Message, ex);
        }

    }
}
