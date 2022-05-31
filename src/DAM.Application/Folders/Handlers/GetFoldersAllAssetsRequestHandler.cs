using DAM.Persistence;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using static DAM.Application.Folders.Requests.FoldersRequest;
using Microsoft.Extensions.Configuration;
using DAM.Application.Services.Interfaces;
using System.Threading;
using System.Linq;
using DAM.Domain.Entities;
using DAM.Application.AuditTrail.Enums;
using DAM.Application.Extensions;
using DAM.Application.Assets.Enums;

namespace DAM.Application.Folders.Handlers
{
    public class GetFoldersAllAssetsRequestHandler : HandlerBase<GetFoldersAllAssetsRequest, HandlerResult<Dictionary<string, Stream>>>
    {
        private readonly IDbContext _dbcontext;
        private readonly IConfiguration _configuration;
        private readonly IAzureStorageService _azureStorageService;
        private readonly IConversionService _conversionService;

        public GetFoldersAllAssetsRequestHandler(IDbContext dbcontext, IConfiguration configuration, IAzureStorageService azureStorageService, IConversionService conversionService)
        {
            _dbcontext = dbcontext ?? throw new ArgumentNullException(nameof(dbcontext));
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
            _azureStorageService = azureStorageService ?? throw new ArgumentNullException(nameof(azureStorageService));
            _conversionService = conversionService ?? throw new ArgumentNullException(nameof(conversionService));
        }

        public override HandlerResult<Dictionary<string, Stream>> HandleRequest(GetFoldersAllAssetsRequest request, CancellationToken cancellationToken, HandlerResult<Dictionary<string, Stream>> result)
        {
            var filenamesAndUrls = new Dictionary<string, Stream>();
            var key = request.UserId;
            // if EmailAddress is null, default user
            if (string.IsNullOrEmpty(key) || key == "null" || key == "public")
            {
                key = _dbcontext.AppUsers.First(u => u.Email == _configuration["DefaultUser"]).Id.ToString();
            }

            int rootFolderId = 0;
            int.TryParse(request.FolderId, out rootFolderId);

            //递归
            var allFolders = _dbcontext.Folders.Where(a => !a.Deleted).ToList();
            var ids = NestedSubFolders(allFolders, request.FolderId);
            ids.Add(rootFolderId);

            if(ids.Count > 0)
            {
                var assetsList = _dbcontext.Assets.Where(a => ids.Contains((int)a.FolderId) && a.Status != Convert.ToInt32(AssetStatus.Archived) && a.Status != Convert.ToInt32(AssetStatus.Deleted)).ToList();
                assetsList.ForEach((asset) =>
                {
                    var assetVersionEntity = _dbcontext.AssetVersions.First(a => a.AssetId == asset.Id && a.ActiveVersion == 1);
                    if (assetVersionEntity != null && assetVersionEntity != null)
                    {
                        int index = assetsList.IndexOf(asset) + 1;
                        var contentType = "";
                        var data = _azureStorageService.GetBlobStream(_configuration, assetVersionEntity.Key + "." + assetVersionEntity.Extension, out contentType);
                        data.Seek(0, SeekOrigin.Begin);

                        var defaultWatermark = _dbcontext.Watermarks.FirstOrDefault(w => !w.CompanyId.HasValue && !w.FolderId.HasValue);

                        if (request.ShowWatermark && defaultWatermark != null && assetVersionEntity.FileType.Contains("image"))
                        {
                            var watermarksm = _azureStorageService.GetWatermarkBlobStream(_configuration, "defaultWatermark.png");

                            var watermarkedOutput = _conversionService.ApplyWatermark(defaultWatermark, data, watermarksm);
                            filenamesAndUrls.Add(index.ToString() + "-" + assetVersionEntity.FileName, watermarkedOutput);
                        }
                        else
                        {
                            filenamesAndUrls.Add(index.ToString() + "-" + assetVersionEntity.FileName, data);
                        }

                        // Add to asset download count 
                        assetVersionEntity.DownloadCount++;
                        _dbcontext.AssetVersions.Update(assetVersionEntity);

                        // Insert into Audit Trail
                        var auditTrailEntry = new AssetAudit()
                        {
                            AssetId = assetVersionEntity.AssetId,
                            AssetFileName = assetVersionEntity.FileName,
                            AuditType = Convert.ToInt32(AssetAuditType.AssetDownloaded),
                            AuditTypeText = AssetAuditType.AssetDownloaded.GetDescription(),
                            AuditCreatedDate = DateTimeOffset.UtcNow,
                            AuditCreatedByName = request.UserId
                        };

                        var auditDisplayUser = _dbcontext.AppUsers.First(u => u.Email == key || u.Id == key);
                        if (auditDisplayUser != null)
                        {
                            auditTrailEntry.AuditCreatedByUserId = auditDisplayUser.Id;
                            auditTrailEntry.AuditCreatedByName = auditDisplayUser.UserName;
                        }

                        _dbcontext.AssetAudit.Add(auditTrailEntry);
                    }
                });

                _dbcontext.SaveChanges();
            }
            result.Entity = filenamesAndUrls;
            result.ResultType = ResultType.Success;
            return result;
        }

        private List<int> NestedSubFolders(List<Folder> folders, string folderId)
        {
            var list = new List<int>();
            foreach(var folder in folders)
            {
                if(folder.ParentFolderId.ToString() == folderId)
                {
                    var subfolderList = NestedSubFolders(folders, folder.Id.ToString());
                    if(subfolderList.Count > 0)
                    {
                        list.AddRange(subfolderList);
                    }
                    list.Add(folder.Id);
                }
                //list.Add(folderId);
            }
            return list;
        }
    }
}
