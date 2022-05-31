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
    public class AddAssetPackageRequestHandler : HandlerBase<AddAssetPackageRequest, HandlerResult<AssetPackageDto>>
    {
        private readonly ICacheProvider _cacheProvider;
        private readonly IDbContext _dbcontext;
        private readonly IMapper _mapper;
        private readonly IConfiguration _configuration;
        private readonly IAzureStorageService _azureStorageService;
        private readonly IConversionService _conversionService;
        private readonly ITagService _tagService;
        private readonly IHelperService _helperService;

        public AddAssetPackageRequestHandler(ICacheProvider cacheProvider, IDbContext dbcontext, IMapper mapper, IConfiguration configuration, IAzureStorageService azureStorageService,
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

        public override HandlerResult<AssetPackageDto> HandleRequest(AddAssetPackageRequest request, CancellationToken cancellationToken, HandlerResult<AssetPackageDto> result)
        {
            var assert = _dbcontext.Assets.FirstOrDefault(v => v.Id == request.AssetPackageDto.AssetId);

            // if createdby is null, default user
            if (string.IsNullOrEmpty(request.UserId))
            {
                assert.ModifiedById = _dbcontext.AppUsers.First(u => u.Email == _configuration["DefaultUser"]).Id;
            }
            else
            {
                assert.ModifiedById = request.UserId;
            }

            BlobProperties ap = new BlobProperties();
            Stream thumbnailStream;

            using (Stream originalStream = new MemoryStream(request.AssetPackageDto.Package))
            {
                thumbnailStream = originalStream;

                if (request.AssetPackageDto.Extension.Contains("zip"))
                {
                    ap = _azureStorageService.UploadFileToStorage(_configuration, originalStream, string.Concat(assert.Id, ".", request.AssetPackageDto.Extension), out Uri assetUri);
                    var original = _azureStorageService.GetBlobSasUri(_configuration, string.Concat(assert.Id, ".", request.AssetPackageDto.Extension), string.Empty, request.AssetPackageDto.FileType, false);
                    assert.ModifiedDate = DateTimeOffset.UtcNow;
                    assert.PackageUrl = original;
                    assert.PackageName = request.AssetPackageDto.PackageName;
                    assert.PackageExtension = request.AssetPackageDto.Extension;

                    _dbcontext.Assets.Update(assert);
                    var auditDisplayUser = _dbcontext.AppUsers.First(u => u.Id == request.UserId);

                    var auditTrailEntry = new AssetAudit()
                    {
                        AssetId = Convert.ToInt32(assert.Id),
                        AssetFileName = assert.PackageName,
                        AuditType = Convert.ToInt32(AssetAuditType.PackageAdded),
                        AuditTypeText = AssetAuditType.PackageAdded.GetDescription(),
                        AuditCreatedByUserId = assert.ModifiedById,
                        AuditCreatedDate = DateTimeOffset.UtcNow,
                        AuditCreatedByName = auditDisplayUser != null ? auditDisplayUser.UserName : "",
                        NewParameters = "Upload new package"
                    };
                    _dbcontext.AssetAudit.Add(auditTrailEntry);
                    _dbcontext.SaveChanges();
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
