using AutoMapper;
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
using System.Linq;
using System.Threading;

namespace DAM.Application.Assets.Handlers
{
    public class UpdateAssetStatusRequestHandler : HandlerBase<UpdateAssetStatusRequest, HandlerResult<UpdateAssetStatusDto>>
    {
        private readonly ICacheProvider _cacheProvider;
        private readonly IDbContext _dbcontext;
        private readonly IMapper _mapper;
        private readonly IConfiguration _configuration;
        private readonly IFeatureFlagService _featureFlagService;
        private readonly IHelperService _helperService;

        public UpdateAssetStatusRequestHandler(ICacheProvider cacheProvider, IDbContext dbcontext, IMapper mapper, IConfiguration configuration, IFeatureFlagService featureFlagService, IHelperService helperService)
        {
            _cacheProvider = cacheProvider ?? throw new ArgumentNullException(nameof(cacheProvider));
            _dbcontext = dbcontext ?? throw new ArgumentNullException(nameof(dbcontext));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
            _featureFlagService = featureFlagService ?? throw new ArgumentNullException(nameof(configuration));
            _helperService = helperService ?? throw new ArgumentNullException(nameof(helperService));
        }

        public override HandlerResult<UpdateAssetStatusDto> HandleRequest(UpdateAssetStatusRequest request, CancellationToken cancellationToken, HandlerResult<UpdateAssetStatusDto> result)
        {
            var assetIdsToUpdateStatus = request.Assets.AssetIds;
            var modifiedBy = request.UserId;
            result.Entity = new UpdateAssetStatusDto();
            result.Entity.IsSuccess = false;

            // If ModifiedBy is null, default user
            if (string.IsNullOrEmpty(modifiedBy))
            {
                modifiedBy = _dbcontext.AppUsers.First(u => u.Email == _configuration["DefaultUser"]).Id;
            } 

            if (assetIdsToUpdateStatus.Count > 0)
            {
                var assetsToUpdateStatus = _dbcontext.Assets.Where(a => assetIdsToUpdateStatus.Contains((int)a.Id))
                    .Include(asset => asset.AssetVersions)
                    .ToList();
               
                var assetsUpdated = new List<int>();

                if (assetsToUpdateStatus.Count > 0)
                {
                    foreach (var asset in assetsToUpdateStatus)
                    {
                        var oldAssetAuditDetails = new AssetAuditDetailDto()
                        {
                            DisplayName = asset.Title,
                            Status = asset.Status.ToString(),
                            ShareFolders = String.IsNullOrEmpty(asset.ShareFolderIds) ? "" : asset.ShareFolderIds
                        };

                        if (request.Assets.Status == Convert.ToInt32(AssetStatus.Archived) 
                            && asset.FolderId != request.Assets.FolderId)
                        {
                            if (!String.IsNullOrEmpty(asset.ShareFolderIds)) {
                                var Aresult = new List<string>();
                                var test = asset.ShareFolderIds.Split(',', StringSplitOptions.RemoveEmptyEntries).ToList();
                                test.ForEach(f =>
                                {
                                    if (f != request.Assets.FolderId.ToString()) 
                                    {
                                        Aresult.Add(f);
                                    }
                                });
                                asset.ShareFolderIds = string.Join(",", Aresult.ToArray()); ;
                                asset.StatusUpdatedDate = DateTimeOffset.UtcNow;
                                asset.ModifiedDate = DateTimeOffset.UtcNow;
                            }
                            else
                            {
                                asset.Status = request.Assets.Status;
                                asset.StatusUpdatedDate = DateTimeOffset.UtcNow;
                                asset.ModifiedDate = DateTimeOffset.UtcNow;
                            }
                        }
                        else
                        { 
                            asset.Status = request.Assets.Status;
                            asset.StatusUpdatedDate = DateTimeOffset.UtcNow;
                            asset.ModifiedDate = DateTimeOffset.UtcNow;
                        }
                        assetsUpdated.Add((int)asset.Id);

                        _dbcontext.Assets.Update(asset);
                        _dbcontext.SaveChanges();
                        // Insert Audit Trail

                        var auditDisplayUser = _dbcontext.AppUsers.First(u => u.Id == modifiedBy);

                        AssetAuditType assetAuditType = AssetAuditType.AssetUpdated;

                        switch (request.Assets.Status)
                        {
                            case (int)AssetStatus.Draft:
                                assetAuditType = AssetAuditType.AssetRestored;
                                break;
                            case (int)AssetStatus.Archived:
                                assetAuditType = AssetAuditType.AssetArchived;
                                break;
                            case (int)AssetStatus.Deleted:
                                assetAuditType = AssetAuditType.AssetDeleted;
                                break;
                        }

                        var newAssetAuditDetails = new AssetAuditDetailDto()
                        {
                            DisplayName = asset.Title,
                            Status = asset.Status.ToString(),
                            ShareFolders = String.IsNullOrEmpty(asset.ShareFolderIds) ? "" : asset.ShareFolderIds
                        };

                        var auditTrailEntry = new AssetAudit()
                        {
                            AssetId = Convert.ToInt32(asset.Id),
                            FolderId = asset.FolderId,
                            AssetFileName = asset.AssetVersions.FirstOrDefault(a=> a.ActiveVersion == 1).FileName,
                            AuditType = Convert.ToInt32(assetAuditType),
                            AuditTypeText = assetAuditType.GetDescription(),
                            AuditCreatedByUserId = asset.ModifiedById,
                            AuditCreatedDate = (DateTimeOffset)(asset.ModifiedDate),
                            AuditCreatedByName = auditDisplayUser != null ? auditDisplayUser.UserName : "",
                            PreviousParameters = _helperService.GetJsonString(oldAssetAuditDetails),
                            NewParameters = _helperService.GetJsonString(newAssetAuditDetails)
                        };

                        _dbcontext.AssetAudit.Add(auditTrailEntry);
                        _dbcontext.SaveChanges();
                    }
                    result.Entity.AssetIds = assetsUpdated;
                    result.Entity.IsSuccess = true;
                }
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