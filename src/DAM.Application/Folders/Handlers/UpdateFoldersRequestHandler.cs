using AutoMapper;
using DAM.Application.Assets.Helpers;
using DAM.Application.AuditTrail.Dtos;
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
    public class UpdateFoldersRequestHandler : HandlerBase<UpdateFolderRequest, HandlerResult<FolderDto>>
    {
        private readonly ICacheProvider _cacheProvider;
        private readonly IDbContext _dbcontext;
        private readonly IMapper _mapper;
        private readonly IConfiguration _configuration;
        private readonly IHelperService _helperService;

        public UpdateFoldersRequestHandler(ICacheProvider cacheProvider, IDbContext dbcontext, IMapper mapper, IConfiguration configuration, IHelperService helperService)
        {
            _cacheProvider = cacheProvider ?? throw new ArgumentNullException(nameof(cacheProvider));
            _dbcontext = dbcontext ?? throw new ArgumentNullException(nameof(dbcontext));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
            _helperService = helperService ?? throw new ArgumentNullException(nameof(helperService));
        }

        public override HandlerResult<FolderDto> HandleRequest(UpdateFolderRequest request, CancellationToken cancellationToken, HandlerResult<FolderDto> result)
        {
            var modifiedBy = request.UserId;

            // If modifiedBy is null, default user
            if (string.IsNullOrEmpty(modifiedBy))
            {
                modifiedBy = _dbcontext.AppUsers.First(u => u.Email == _configuration["DefaultUser"]).Id;
            }

            var folder = _dbcontext.Folders.First(f => f.Id == request.FolderDto.Id);

            if (folder != null)
            {
                // Retrieve existing details for audit transaction
                var oldFolderAccounts = _dbcontext.FolderAccounts.Where(x => x.FolderId == folder.Id).Include(aa => aa.Account);
                var oldFolderRegions = _dbcontext.FolderCountryRegions.Where(x => x.FolderId == folder.Id).Include(acr => acr.Region).Include(r => r.Country);
                var oldFolder = folder.ParentFolderId == null ? "" : _dbcontext.Folders.First(f => f.Id == folder.ParentFolderId).FolderName;

                var oldAccounts = oldFolderAccounts.Select(a => a.Account.Description).ToList();
                var oldRegions = oldFolderRegions.Select(ar => ar.Region.Description).ToList();
                var oldCountries = oldFolderRegions.Select(ar => ar.Country.Name).Distinct().ToList();

                var oldFolderAuditDetails = new FolderAuditDetailDto()
                {
                    DisplayName = folder.FolderName,
                    Description = folder.Description,
                    Accounts = oldAccounts,
                    Countries = oldCountries,
                    Regions = oldRegions,
                    Folder = oldFolder
                };

                if (request.FolderDto.ParentFolderId == 0)
                {
                    folder.ParentFolderId = null;
                }

                folder.FolderName = request.FolderDto.FolderName;
                folder.Description = request.FolderDto.Comments;
                folder.ModifiedById = modifiedBy;
                folder.ModifiedDate = DateTimeOffset.UtcNow;

                _dbcontext.Folders.Update(folder);

                var accounts = request.FolderDto.Accounts;

                // Add account metadata
                if (accounts != null)
                {
                    //Override current accounts
                    var folderAccounts = oldFolderAccounts;

                    _dbcontext.FolderAccounts.RemoveRange(folderAccounts);

                    // asset has own specified accounts 
                    foreach (var account in accounts)
                    {
                        _dbcontext.FolderAccounts.Add(new FolderAccountMetaData()
                        {
                            FolderId = (int)(folder.Id),
                            AccountId = account.Id
                        });
                    }
                }

                // Add country/region metadata
                var regions = request.FolderDto.Regions;
                if (regions != null)
                {
                    //Override current countries and regions
                    var folderRegions = oldFolderRegions;

                    _dbcontext.FolderCountryRegions.RemoveRange(folderRegions);

                    // asset has own specified country/region 
                    foreach (var region in regions)
                    {
                        _dbcontext.FolderCountryRegions.Add(new FolderCountryRegionMetaData()
                        {
                            FolderId = (int)(folder.Id),
                            RegionId = region.Id,
                            CountryId = region.CountryId
                        });
                    }
                }

                _dbcontext.SaveChanges();

                // Insert into Audit Trail
                var newAccounts = request.FolderDto.Accounts == null ? oldAccounts : request.FolderDto.Accounts.Select(a => a.Name).ToList();
                var newCountries = request.FolderDto.Countries == null ? oldCountries : request.FolderDto.Countries.Select(c => c.Name).ToList();
                var newRegions = request.FolderDto.Regions == null ? oldRegions : request.FolderDto.Regions.Select(r => r.Description).ToList();

                var newFolderAuditDetails = new FolderAuditDetailDto()
                {
                    DisplayName = folder.FolderName,
                    Description = folder.Description,
                    Accounts = newAccounts,
                    Countries = newCountries,
                    Regions = newRegions,
                    Folder = oldFolder
                };

                var auditDisplayUser = _dbcontext.AppUsers.First(u => u.Id == folder.ModifiedById);

                var auditTrailEntry = new AssetAudit()
                {
                    FolderId = Convert.ToInt32(folder.Id),
                    FolderName = folder.FolderName,
                    AuditType = Convert.ToInt32(AssetAuditType.FolderUpdated),
                    AuditTypeText = AssetAuditType.FolderUpdated.GetDescription(),
                    AuditCreatedByUserId = folder.ModifiedById,
                    AuditCreatedDate = (DateTimeOffset)(folder.ModifiedDate),
                    AuditCreatedByName = auditDisplayUser != null ? auditDisplayUser.UserName : "",
                    PreviousParameters = _helperService.GetJsonString(oldFolderAuditDetails),
                    NewParameters = _helperService.GetJsonString(newFolderAuditDetails)
                };

                _dbcontext.AssetAudit.Add(auditTrailEntry);
                _dbcontext.SaveChanges();

                result.Entity = _mapper.Map<FolderDto>(folder);
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
