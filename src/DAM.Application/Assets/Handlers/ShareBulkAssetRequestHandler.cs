using System;
using DAM.Application.Assets.Dtos;
using DAM.Application.Assets.Requests;
using DAM.Application.Cache;
using DAM.Persistence;
using AutoMapper;
using Microsoft.Extensions.Configuration;
using System.Threading;
using System.Linq;
using DAM.Application.Services.Interfaces;
using DAM.Application.Assets.Enums;
using System.Collections.Generic;

namespace DAM.Application.Assets.Handlers
{
    public class ShareBulkAssetRequestHandler : HandlerBase<ShareBulkAssetRequest, HandlerResult<string>>
    {
        private readonly IDbContext _dbcontext;
        private readonly IConfiguration _configuration;

        public ShareBulkAssetRequestHandler(ICacheProvider cacheProvider, IDbContext dbcontext, IMapper mapper, IConfiguration configuration, IEmailService emailService)
        {
            _dbcontext = dbcontext ?? throw new ArgumentNullException(nameof(dbcontext));
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
        }

        public override HandlerResult<string> HandleRequest(ShareBulkAssetRequest request, CancellationToken cancellationToken, HandlerResult<string> result)
        {
            var assetIdList = request.AssetsIds.Split(",").ToList();
            assetIdList.ForEach((id) =>
            {
                int assetId = 0;
                int.TryParse(id, out assetId);
                if(assetId > 0)
                { 
                    var asset = _dbcontext.Assets.FirstOrDefault(a => a.Status != Convert.ToInt32(AssetStatus.Archived) && a.Status != Convert.ToInt32(AssetStatus.Deleted) && a.Id == assetId);

                    if (asset != null)
                    {
                        if (string.IsNullOrEmpty(asset.ShareFolderIds))
                        {
                            var shareIds = "";
                            var shareFolderIds = new List<string>();
                            request.FolderIds.Split(',').ToList().ForEach((id) => { if (id != asset.FolderId.ToString()) { shareFolderIds.Add(id); } });
                            shareIds = string.Join(",", shareFolderIds);
                            asset.ShareFolderIds = shareIds;
                        }
                        else
                        {
                            var shareIds = "";
                            var shareFolderIds = new List<string>();
                            request.FolderIds.Split(',').ToList().ForEach((id) => { if (id != asset.FolderId.ToString()) { shareFolderIds.Add(id); } });
                            var tempList = asset.ShareFolderIds.Split(',').ToList();
                            tempList.AddRange(shareFolderIds);
                            var newFolderIdList = tempList.Distinct().ToList();
                            shareIds = string.Join(",", newFolderIdList);
                            asset.ShareFolderIds = shareIds;
                        }
                        _dbcontext.Assets.Update(asset);
                    }
                }
            });
            _dbcontext.SaveChanges();
            result.ResultType = ResultType.Success;
            result.Entity = string.Concat("Asstes:", request.AssetsIds, ",", "Folders:", request.FolderIds);
            return result;
        }

        public override Exception OnException(Exception ex)
        {
            return new Exception(ex.Message, ex);
        }
    }
}
