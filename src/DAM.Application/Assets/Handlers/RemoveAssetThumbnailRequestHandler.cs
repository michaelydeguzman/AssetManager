using AutoMapper;
using Azure.Storage.Blobs.Models;
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
using Force.Crc32;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading;

namespace DAM.Application.Assets.Handlers
{
    public class RemoveAssetThumbnailRequestHandler : HandlerBase<RemoveAssetThumbnailRequest, HandlerResult<AssetThumbnailDto>>
    {
        private readonly ICacheProvider _cacheProvider;
        private readonly IDbContext _dbcontext;
        private readonly IMapper _mapper;
        private readonly IConfiguration _configuration;
        private readonly IAzureStorageService _azureStorageService;
        private readonly IConversionService _conversionService;
        private readonly ITagService _tagService;
        private readonly IHelperService _helperService;

        public RemoveAssetThumbnailRequestHandler(ICacheProvider cacheProvider, IDbContext dbcontext, IMapper mapper, IConfiguration configuration, IAzureStorageService azureStorageService,
           IConversionService conversionService, ITagService tagService, IHelperService helperService)
        {
            _cacheProvider = cacheProvider ?? throw new ArgumentNullException(nameof(cacheProvider));
            _dbcontext = dbcontext ?? throw new ArgumentNullException(nameof(dbcontext));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
            _azureStorageService = azureStorageService ?? throw new ArgumentNullException(nameof(azureStorageService));
            _conversionService = conversionService ?? throw new ArgumentNullException(nameof(conversionService));
            _tagService = tagService ?? throw new ArgumentNullException(nameof(tagService));
            _helperService = helperService ?? throw new ArgumentNullException(nameof(helperService));
        }

        public override HandlerResult<AssetThumbnailDto> HandleRequest(RemoveAssetThumbnailRequest request, CancellationToken cancellationToken, HandlerResult<AssetThumbnailDto> result)
        {
            var version = _dbcontext.AssetVersions.FirstOrDefault(v => v.AssetId == request.AssetThumbnailDto.AssetId && v.ActiveVersion == 1);

            // if createdby is null, default user
            if (string.IsNullOrEmpty(request.UserId))
            {
                version.ModifiedById = _dbcontext.AppUsers.First(u => u.Email == _configuration["DefaultUser"]).Id;
            }
            else
            {
                version.ModifiedById = request.UserId;
            }

            version.ModifiedDate = DateTimeOffset.UtcNow;
            version.ThumbnailPreview = "";
            _dbcontext.AssetVersions.Update(version);
            var auditDisplayUser = _dbcontext.AppUsers.First(u => u.Id == request.UserId);

            var auditTrailEntry = new AssetAudit()
            {
                AssetId = Convert.ToInt32(version.AssetId),
                AssetFileName = version.FileName,
                AuditType = Convert.ToInt32(AssetAuditType.RemoveAssetPreview),
                AuditTypeText = AssetAuditType.RemoveAssetPreview.GetDescription(),
                AuditCreatedByUserId = version.ModifiedById,
                AuditCreatedDate = DateTimeOffset.UtcNow,
                AuditCreatedByName = auditDisplayUser != null ? auditDisplayUser.UserName : "",
                NewParameters = "Remove thumbnail"
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
