using System;
using DAM.Application.Assets.Requests;
using DAM.Application.Cache;
using DAM.Persistence;
using AutoMapper;
using Microsoft.Extensions.Configuration;
using System.Threading;
using System.Linq;
using System.IO;
using DAM.Domain.Entities;
using DAM.Application.AuditTrail.Enums;

using DAM.Application.Services.Interfaces;
using DAM.Application.Extensions;
using System.Collections.Generic;

namespace DAM.Application.Assets.Handlers
{
    public class DownloadBulkAssetsRequestHandler : HandlerBase<DownloadBulkAssetsRequest, HandlerResult<Dictionary<string, Stream>>>
    {

        private readonly IDbContext _dbcontext;
        private readonly IConfiguration _configuration;
        private readonly IAzureStorageService _azureStorageService;
        private readonly IConversionService _conversionService;

        public DownloadBulkAssetsRequestHandler(IDbContext dbcontext, IConfiguration configuration, IAzureStorageService azureStorageService, IConversionService conversionService)
        {
            _dbcontext = dbcontext ?? throw new ArgumentNullException(nameof(dbcontext));
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
            _azureStorageService = azureStorageService ?? throw new ArgumentNullException(nameof(azureStorageService));
            _conversionService = conversionService ?? throw new ArgumentNullException(nameof(conversionService));
        }

        public override HandlerResult<Dictionary<string, Stream>> HandleRequest(DownloadBulkAssetsRequest request, CancellationToken cancellationToken, HandlerResult<Dictionary<string, Stream>> result)
        {
            var filenamesAndUrls = new Dictionary<string, Stream>();
            var key = request.EmailAddress;
            // if EmailAddress is null, default user
            if (string.IsNullOrEmpty(key) || key == "null" || key == "public")
            {
                key = _dbcontext.AppUsers.First(u => u.Email == _configuration["DefaultUser"]).Id.ToString();
            }

            var assetsList = request.AssetsIds.Split(",").ToList();
            assetsList.ForEach((asset) =>
            {
                int assetId = 0;
                int.TryParse(asset, out assetId);
                if(assetId > 0)
                { 
                    var assetVersionEntity = _dbcontext.AssetVersions.First(a => a.AssetId == assetId && a.ActiveVersion == 1);

                    if (assetVersionEntity != null && assetVersionEntity != null)
                    {

                        int index = assetsList.IndexOf(asset) + 1;
                        var contentType = "";
                        var data = _azureStorageService.GetBlobStream(_configuration, assetVersionEntity.Key + "." + assetVersionEntity.Extension, out contentType);
                        data.Seek(0, SeekOrigin.Begin);

                        var defaultWatermark = _dbcontext.Watermarks.FirstOrDefault(w => !w.CompanyId.HasValue && !w.FolderId.HasValue);

                        if (request.ShowWaterMark && defaultWatermark != null && assetVersionEntity.FileType.Contains("image"))
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
                            AuditCreatedByName = request.EmailAddress
                        };

                        var auditDisplayUser = _dbcontext.AppUsers.First(u => u.Email == key || u.Id == key);
                        if (auditDisplayUser != null)
                        {
                            auditTrailEntry.AuditCreatedByUserId = auditDisplayUser.Id;
                            auditTrailEntry.AuditCreatedByName = auditDisplayUser.UserName;
                        }

                        _dbcontext.AssetAudit.Add(auditTrailEntry);
                    }
                }
            });

            _dbcontext.SaveChanges();
            result.Entity = filenamesAndUrls;
            result.ResultType = ResultType.Success;
            return result;
        }

        public override Exception OnException(Exception ex)
        {
            return new Exception(ex.Message, ex);
        }
    }
}
