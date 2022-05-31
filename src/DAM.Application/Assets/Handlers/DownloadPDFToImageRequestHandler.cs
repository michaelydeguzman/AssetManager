using System;
using DAM.Application.Assets.Dtos;
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
using DAM.Application.Assets.Enums;
using DAM.Application.Services.Interfaces;
using DAM.Application.Extensions;
using System.Collections.Generic;
using System.Net.Http;

namespace DAM.Application.Assets.Handlers
{
    public class DownloadPDFToImageRequestHandler : HandlerBase<DownloadPDFToImageRequest, HandlerResult<Dictionary<string, Stream>>>
    {

        private readonly ICacheProvider _cacheProvider;
        private readonly IDbContext _dbcontext;
        private readonly IMapper _mapper;
        private readonly IConfiguration _configuration;
        private readonly IAzureStorageService _azureStorageService;
        private readonly IConversionService _conversionService;

        public DownloadPDFToImageRequestHandler(ICacheProvider cacheProvider, IDbContext dbcontext, IMapper mapper, IConfiguration configuration,
            IAzureStorageService azureStorageService, IConversionService conversionService)
        {
            _cacheProvider = cacheProvider ?? throw new ArgumentNullException(nameof(cacheProvider));
            _dbcontext = dbcontext ?? throw new ArgumentNullException(nameof(dbcontext));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
            _azureStorageService = azureStorageService ?? throw new ArgumentNullException(nameof(azureStorageService));
            _conversionService = conversionService ?? throw new ArgumentNullException(nameof(conversionService));
        }

        public override HandlerResult<Dictionary<string, Stream>> HandleRequest(DownloadPDFToImageRequest request, CancellationToken cancellationToken, HandlerResult<Dictionary<string, Stream>> result)
        {
            AssetVersions assetEntity = new AssetVersions();

            int assetId = 0;
            int.TryParse(request.ShareDto.AssetKey, out assetId);
            if (assetId > 0)
            {
                assetEntity = _dbcontext.AssetVersions.First(a => a.AssetId == assetId && a.ActiveVersion == 1);
            }
            else
            {
                //download the specific version of the asset
                assetEntity = _dbcontext.AssetVersions.First(a => a.Key == request.ShareDto.AssetKey && a.Status != Convert.ToInt32(AssetStatus.Deleted));
            }

            var key = request.ShareDto.EmailAddress;

            // if EmailAddress is null, default user
            if (string.IsNullOrEmpty(key) || key == "null" || key == "public")
            {
                key = _dbcontext.AppUsers.First(u => u.Email == _configuration["DefaultUser"]).Id.ToString();
            }

            if (assetEntity == null)
            {
                result.ResultType = ResultType.NoData;
                return result;
            }

            var contentType = "";

            Stream data;
            Stream watermarksm;

            var outputListms = new Dictionary<string, Stream>();

            data = _azureStorageService.GetBlobStream(_configuration, assetEntity.Key + "." + assetEntity.Extension, out contentType);
            data.Seek(0, SeekOrigin.Begin);

            var defaultWatermark = _dbcontext.Watermarks.FirstOrDefault(w => !w.CompanyId.HasValue && !w.FolderId.HasValue);

            if (assetEntity.Extension == "pdf" && request.ShareDto.Extension == "png") // pdf to png
            {
                if (request.ShareDto.ShowWaterMark && defaultWatermark != null)
                {
                    watermarksm = _azureStorageService.GetWatermarkBlobStream(_configuration, "defaultWatermark.png");
                    outputListms = _conversionService.ConvertPDFPagesToPNG(data, assetEntity.FileName.Replace("pdf", request.ShareDto.Extension), request.ShareDto.ShowWaterMark, watermarksm, defaultWatermark);
                }
                else
                {
                    outputListms = _conversionService.ConvertPDFPagesToPNG(data, assetEntity.FileName.Replace("pdf", request.ShareDto.Extension));
                }
               
            }
            else if (assetEntity.Extension == "pdf" && (request.ShareDto.Extension == "jpeg" || request.ShareDto.Extension == "jpg")) // pdf to jpeg
            {
                if (request.ShareDto.ShowWaterMark && defaultWatermark != null)
                {
                    watermarksm = _azureStorageService.GetWatermarkBlobStream(_configuration, "defaultWatermark.png");
                    outputListms = _conversionService.ConvertPDFPagesToJpeg(data, assetEntity.FileName.Replace("pdf", "jpeg"), request.ShareDto.ShowWaterMark, watermarksm, defaultWatermark);
                } 
                else
                {
                    outputListms = _conversionService.ConvertPDFPagesToJpeg(data, assetEntity.FileName.Replace("pdf", "jpeg"));
                }
            }

            result.Entity = outputListms;
            result.ResultType = ResultType.Success;

            // Add to asset download count 
            assetEntity.DownloadCount++;
            _dbcontext.AssetVersions.Update(assetEntity);

            // Insert into Audit Trail
            var auditTrailEntry = new AssetAudit()
            {
                AssetId = assetEntity.AssetId,
                AssetFileName = assetEntity.FileName,
                AuditType = Convert.ToInt32(AssetAuditType.AssetDownloaded),
                AuditTypeText = AssetAuditType.AssetDownloaded.GetDescription(),
                AuditCreatedDate = DateTimeOffset.UtcNow,
                AuditCreatedByName = key
            };

            var auditDisplayUser = _dbcontext.AppUsers.First(u => u.Email == key || u.Id == key);
            if (auditDisplayUser != null)
            {
                auditTrailEntry.AuditCreatedByUserId = auditDisplayUser.Id;
                auditTrailEntry.AuditCreatedByName = auditDisplayUser.UserName;
            }

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
