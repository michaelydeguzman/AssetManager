using AutoMapper;
using DAM.Application.AuditTrail.Dtos;
using DAM.Application.Cache;
using DAM.Application.Folders.Dtos;
using DAM.Domain.Entities;
using DAM.Persistence;
using Microsoft.Extensions.Configuration;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading;
using static DAM.Application.Folders.Requests.FoldersRequest;
using DAM.Application.AuditTrail.Enums;

using DAM.Application.Services.Interfaces;
using DAM.Application.Extensions;

namespace DAM.Application.Folders.Handlers
{
    public class AddFoldersRequestHandler : HandlerBase<AddFolderRequest, HandlerResult<FolderDto>>
    {
        private readonly ICacheProvider _cacheProvider;
        private readonly IDbContext _dbcontext;
        private readonly IMapper _mapper;
        private readonly IConfiguration _configuration;
        private readonly IHelperService _helperService;

        public AddFoldersRequestHandler(ICacheProvider cacheProvider, IDbContext dbcontext, IMapper mapper, IConfiguration configuration, IHelperService helperService)
        {
            _cacheProvider = cacheProvider ?? throw new ArgumentNullException(nameof(cacheProvider));
            _dbcontext = dbcontext ?? throw new ArgumentNullException(nameof(dbcontext));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
            _helperService = helperService ?? throw new ArgumentNullException(nameof(helperService));
        }

        public override HandlerResult<FolderDto> HandleRequest(AddFolderRequest request, CancellationToken cancellationToken, HandlerResult<FolderDto> result)
        {
            var folder = _mapper.Map<Folder>(request.FolderDto);
            var createdBy = request.UserId;

            // If CreatedBy is null, default user
            if (string.IsNullOrEmpty(createdBy))
            {
                folder.CreatedById = _dbcontext.AppUsers.First(u => u.Email == _configuration["DefaultUser"]).Id;
            }
            else
            {
                folder.CreatedById = createdBy;
            }

            folder.Deleted = false;
            if (folder.ParentFolderId == 0)
            {
                folder.ParentFolderId = null;
            }
            var accounts = request.FolderDto.Accounts;
            if (request.FolderDto.ParentFolderId == 0)
            {
                folder.ParentFolderId = null;
            }
            bool hasFolder = folder.ParentFolderId != null;

            _dbcontext.Folders.Add(folder);
            _dbcontext.SaveChanges();

            // Add account metadata
            if (accounts != null)
            {
                // asset has own specified accounts 
                foreach (var account in accounts)
                {
                    _dbcontext.FolderAccounts.Add(new FolderAccountMetaData()
                    {
                        FolderId = (int)(folder.Id),
                        AccountId = (int)(account.Id)
                    });
                }
            }
            else
            {
                if (hasFolder)
                {
                    // check if folder has account metadata mapping, if yes, apply to asset as well
                    var folderAccountIds = _dbcontext.FolderAccounts.Where(f => f.FolderId == folder.ParentFolderId).Select(i => i.AccountId).ToList();

                    foreach (var accountId in folderAccountIds)
                    {
                        _dbcontext.FolderAccounts.Add(new FolderAccountMetaData()
                        {
                            FolderId = (int)(folder.Id),
                            AccountId = accountId
                        });
                    }
                }
            }

            // Add country/region metadata
            var regions = request.FolderDto.Regions;
            if (regions != null)
            {
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
            else
            {
                if (hasFolder)
                {
                    // check if folder has region metadata mapping, if yes, apply to asset as well
                    var parentFolderRegions = _dbcontext.FolderCountryRegions.Where(f => f.FolderId == folder.ParentFolderId).ToList();

                    foreach (var region in parentFolderRegions)
                    {
                        _dbcontext.FolderCountryRegions.Add(new FolderCountryRegionMetaData()
                        {
                            FolderId = (int)(folder.Id),
                            RegionId = (int)(region.Id),
                            CountryId = region.CountryId
                        });
                    }
                }
            }

            _dbcontext.SaveChanges();
            result.Entity = _mapper.Map<FolderDto>(folder);

            var folderAccounts = _dbcontext.FolderAccounts.Where(x => x.FolderId == folder.Id).Include(aa => aa.Account);
            var folderRegions = _dbcontext.FolderCountryRegions.Where(x => x.FolderId == folder.Id).Include(acr => acr.Region).Include(r => r.Country);

            // Insert into Audit Trail
            var folderAuditDetail = new FolderAuditDetailDto()
            {
                DisplayName = folder.FolderName,
                Description = null,
                Accounts = folderAccounts.Select(fa => fa.Account.Description).ToList(),
                Countries = folderRegions.Select(fcr => fcr.Country.Name).ToList(),
                Regions = folderRegions.Select(fcr => fcr.Region.Description).ToList(),
                Folder = hasFolder ? _dbcontext.Folders.First(f => f.Id == folder.ParentFolderId).FolderName : null
            };

            var auditDisplayUser = _dbcontext.AppUsers.First(u => u.Id == createdBy);

            var auditTrailEntry = new AssetAudit()
            {
                FolderId = Convert.ToInt32(folder.Id),
                FolderName = folder.FolderName,
                AuditType = Convert.ToInt32(AssetAuditType.FolderCreated),
                AuditTypeText = AssetAuditType.FolderCreated.GetDescription(),
                AuditCreatedByUserId = folder.CreatedById,
                AuditCreatedDate = folder.CreatedDate,
                AuditCreatedByName = auditDisplayUser != null ? auditDisplayUser.UserName : "",
                NewParameters = _helperService.GetJsonString(folderAuditDetail)
            };

            _dbcontext.AssetAudit.Add(auditTrailEntry);
            _dbcontext.SaveChanges();
            result.ResultType = ResultType.Success;
            return result;
        }

        public override Exception OnException(Exception ex)
        {
            return new Exception(ex.Message, ex);
        }

    }
}
