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
    public class DownloadOfficeToPDFRequestHandler : HandlerBase<DownloadOfficeToPDFRequest, HandlerResult<HttpResponseMessage>>
    {

        private readonly ICacheProvider _cacheProvider;
        private readonly IDbContext _dbcontext;
        private readonly IMapper _mapper;
        private readonly IConfiguration _configuration;
        private readonly IAzureStorageService _azureStorageService;
        private readonly IConversionService _conversionService;

        public DownloadOfficeToPDFRequestHandler(ICacheProvider cacheProvider, IDbContext dbcontext, IMapper mapper, IConfiguration configuration,
            IAzureStorageService azureStorageService, IConversionService conversionService)
        {
            _cacheProvider = cacheProvider ?? throw new ArgumentNullException(nameof(cacheProvider));
            _dbcontext = dbcontext ?? throw new ArgumentNullException(nameof(dbcontext));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
            _azureStorageService = azureStorageService ?? throw new ArgumentNullException(nameof(azureStorageService));
            _conversionService = conversionService ?? throw new ArgumentNullException(nameof(conversionService));
        }

        public override HandlerResult<HttpResponseMessage> HandleRequest(DownloadOfficeToPDFRequest request, CancellationToken cancellationToken, HandlerResult<HttpResponseMessage> result)
        {
            AssetVersions assetEntity = new AssetVersions();
            int assetId = 0;
            int.TryParse(request.AssetKey, out assetId);

            if (assetId > 0)
            {
                assetEntity = _dbcontext.AssetVersions.First(a => a.AssetId == assetId && a.ActiveVersion == 1);
            }
            else
            {
                //download the specific version of the asset
                assetEntity = _dbcontext.AssetVersions.First(a => a.Key == request.AssetKey && a.Status != Convert.ToInt32(AssetStatus.Deleted));
            }

            var key = request.EmailAddress;

            // if EmailAddress is null, default user
            if (string.IsNullOrEmpty(key) || key == "null" || key == "public")
            {
                key = _dbcontext.AppUsers.First(u => u.Email == _configuration["DefaultUser"]).Id.ToString();
            }

            Stream data;
            var contentType = "";

            data = _azureStorageService.GetBlobStream(_configuration, assetEntity.Key + "." + assetEntity.Extension, out contentType);

            result.Entity = _conversionService.ConvertOfficeToPDF(_configuration, data, assetEntity.FileName, assetEntity.Extension);

            if (assetEntity == null)
            {
                result.ResultType = ResultType.NoData;
                return result;
            }

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
