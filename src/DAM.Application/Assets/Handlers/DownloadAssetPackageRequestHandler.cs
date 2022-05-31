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
    public class DownloadAssetPackageRequestHandler : HandlerBase<DownloadAssetPackageRequest, HandlerResult<DownloadDto>>
    {

        private readonly ICacheProvider _cacheProvider;
        private readonly IDbContext _dbcontext;
        private readonly IMapper _mapper;
        private readonly IConfiguration _configuration;
        private readonly IAzureStorageService _azureStorageService;
        private readonly IConversionService _conversionService;

        public DownloadAssetPackageRequestHandler(ICacheProvider cacheProvider, IDbContext dbcontext, IMapper mapper, IConfiguration configuration,
            IAzureStorageService azureStorageService, IConversionService conversionService)
        {
            _cacheProvider = cacheProvider ?? throw new ArgumentNullException(nameof(cacheProvider));
            _dbcontext = dbcontext ?? throw new ArgumentNullException(nameof(dbcontext));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
            _azureStorageService = azureStorageService ?? throw new ArgumentNullException(nameof(azureStorageService));
            _conversionService = conversionService ?? throw new ArgumentNullException(nameof(conversionService));
        }

        public override HandlerResult<DownloadDto> HandleRequest(DownloadAssetPackageRequest request, CancellationToken cancellationToken, HandlerResult<DownloadDto> result)
        {
            Asset assetEntity = new Asset();
            int assetId = 0;
            int.TryParse(request.ShareDto.AssetKey, out assetId);

            if (assetId > 0)
            {
                assetEntity = _dbcontext.Assets.First(a => a.Id == assetId);
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
                result.Entity = new DownloadDto()
                {
                    Content = new MemoryStream(),
                    FileName = "",
                    ContentType = ""
                };

                var contentType = "";
                Stream data;
                var outputListms = new List<Stream>();

                data = _azureStorageService.GetBlobStream(_configuration, assetEntity.Id + "." + assetEntity.PackageExtension, out contentType);

                data.Seek(0, SeekOrigin.Begin);
                data.CopyTo(result.Entity.Content);
                result.Entity.Content.Seek(0, SeekOrigin.Begin);
                result.Entity.ContentType = contentType;
                result.Entity.FileName = assetEntity.PackageName;
                
                // Insert into Audit Trail
                var auditTrailEntry = new AssetAudit()
                {
                    AssetId = assetEntity.Id,
                    AssetFileName = assetEntity.PackageName,
                    AuditType = Convert.ToInt32(AssetAuditType.AssetPackageDownloaded),
                    AuditTypeText = AssetAuditType.AssetPackageDownloaded.GetDescription(),
                    AuditCreatedDate = DateTimeOffset.UtcNow,
                    AuditCreatedByName = key
                };

                var auditDisplayUser = _dbcontext.AppUsers.First(u => u.Id == request.UserId);
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
            else
            {
                result.ResultType = ResultType.NoData;
                return result;
            }     
        }

        public override Exception OnException(Exception ex)
        {
            return new Exception(ex.Message, ex);
        }


    }
}
