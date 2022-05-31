using AutoMapper;
using DAM.Application.Assets.Dtos;
using DAM.Application.Assets.Requests;
using DAM.Application.AuditTrail.Enums;

using DAM.Application.Cache;
using DAM.Application.Extensions;
using DAM.Domain.Entities;
using DAM.Persistence;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace DAM.Application.Assets.Handlers
{
    public class MoveAssetRequestHandler : HandlerBase<MoveAssetsRequest, HandlerResult<MoveAssetsDto>>
    {
        private readonly ICacheProvider _cacheProvider;
        private readonly IDbContext _dbcontext;
        private readonly IMapper _mapper;
        private readonly IConfiguration _configuration;

        public MoveAssetRequestHandler(ICacheProvider cacheProvider, IDbContext dbcontext, IMapper mapper, IConfiguration configuration)
        {
            _cacheProvider = cacheProvider ?? throw new ArgumentNullException(nameof(cacheProvider));
            _dbcontext = dbcontext ?? throw new ArgumentNullException(nameof(dbcontext));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
        }

        public override HandlerResult<MoveAssetsDto> HandleRequest(MoveAssetsRequest request, CancellationToken cancellationToken, HandlerResult<MoveAssetsDto> result)
        {
            var newFolderId = request.MoveAssets.FolderId;
            var modifiedBy = request.UserId;

            // If modifiedBy is null, default user
            if (string.IsNullOrEmpty(modifiedBy))
            {
                modifiedBy = _dbcontext.AppUsers.First(u => u.Email == _configuration["DefaultUser"]).Id;
            }

            foreach (var assetId in request.MoveAssets.AssetIds)
            {
                var asset = _dbcontext.Assets.First(a => a.Id == assetId);
                var version = _dbcontext.AssetVersions.FirstOrDefault(a => a.AssetId == assetId && a.ActiveVersion == 1);


                if (asset.Id != 0)
                {
                    var oldFolder = _dbcontext.Folders.First(f => f.Id == asset.FolderId).FolderName;
                    var newFolder = _dbcontext.Folders.First(f => f.Id == newFolderId).FolderName;

                    asset.FolderId = newFolderId == 0 ? null : newFolderId;
                    asset.ModifiedDate = DateTimeOffset.UtcNow;
                    asset.ModifiedById = modifiedBy;
                    if (!String.IsNullOrEmpty(asset.ShareFolderIds))
                    {
                        var shareFolderIds = new List<string>();
                        asset.ShareFolderIds.Split(',').ToList().ForEach((id) => { if (id != newFolderId.ToString()) { shareFolderIds.Add(id); } });
                        var shareIds = string.Join(",", shareFolderIds);
                        asset.ShareFolderIds = shareIds;
                    }
                    _dbcontext.Assets.Update(asset);

                    var auditDisplayUser = _dbcontext.AppUsers.First(u => u.Id == modifiedBy);

                    var auditTrailEntry = new AssetAudit()
                    {
                        AssetId = Convert.ToInt32(asset.Id),
                        AssetFileName = version.FileName,
                        AuditType = Convert.ToInt32(AssetAuditType.AssetMoved),
                        AuditTypeText = AssetAuditType.AssetMoved.GetDescription(),
                        AuditCreatedByUserId = asset.ModifiedById,
                        AuditCreatedDate = (DateTimeOffset)(asset.ModifiedDate),
                        AuditCreatedByName = auditDisplayUser != null ? auditDisplayUser.UserName : "",
                        PreviousParameters = $"Folder: {oldFolder}",
                        NewParameters = $"Folder: {newFolder}",
                        FolderId = asset.FolderId
                    };

                    _dbcontext.AssetAudit.Add(auditTrailEntry);
                    _dbcontext.SaveChanges();
                }
            }

            result.Entity = request.MoveAssets;
            result.ResultType = ResultType.Success;

            return result;
        }

        public override Exception OnException(Exception ex)
        {
            return new Exception(ex.Message, ex);
        }

    }
}