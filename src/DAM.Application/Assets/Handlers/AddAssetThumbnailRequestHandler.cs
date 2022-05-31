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
    public class AddAssetThumbnailRequestHandler : HandlerBase<AddAssetThumbnailRequest, HandlerResult<AssetThumbnailDto>>
    {
        private readonly ICacheProvider _cacheProvider;
        private readonly IDbContext _dbcontext;
        private readonly IMapper _mapper;
        private readonly IConfiguration _configuration;
        private readonly IAzureStorageService _azureStorageService;
        private readonly IConversionService _conversionService;
        private readonly ITagService _tagService;
        private readonly IHelperService _helperService;

        public AddAssetThumbnailRequestHandler(ICacheProvider cacheProvider, IDbContext dbcontext, IMapper mapper, IConfiguration configuration, IAzureStorageService azureStorageService,
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

        public override HandlerResult<AssetThumbnailDto> HandleRequest(AddAssetThumbnailRequest request, CancellationToken cancellationToken, HandlerResult<AssetThumbnailDto> result)
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

            BlobProperties ap = new BlobProperties();
            var thumbnail = "";
            Stream thumbnailStream;

            using (Stream originalStream = new MemoryStream(request.AssetThumbnailDto.Thumbnail))
            {
                thumbnailStream = originalStream;
                bool convertThumbnailsuccess = false;

               if (request.AssetThumbnailDto.Extension.Contains("pdf"))
               {
                    try
                    {
                        thumbnailStream = _conversionService.GetPDFThumbnail(_configuration, originalStream);
                        convertThumbnailsuccess = true;
                    }
                    catch (Exception ex)
                    {
                        convertThumbnailsuccess = false;
                    }
                }
                if (convertThumbnailsuccess)
                {
                    thumbnailStream = _conversionService.GetImageThumbnail(_configuration, thumbnailStream, request.AssetThumbnailDto.Extension);
                    BlobProperties thumbnailProps = _azureStorageService.UploadFileToStorage(_configuration, thumbnailStream, string.Concat(version.Key, ".", request.AssetThumbnailDto.Extension), out Uri thumbnailUri, true);
                    thumbnail = _azureStorageService.GetBlobSasUri(_configuration, string.Concat(version.Key, ".", request.AssetThumbnailDto.Extension), string.Empty, string.Empty, true);
                }
                else if (request.AssetThumbnailDto.FileType.Contains("image"))
                {
                    // TIFF
                    if (request.AssetThumbnailDto.Extension.Contains("tiff") || request.AssetThumbnailDto.Extension.Contains("tif"))
                    {
                        thumbnailStream = _conversionService.GetImageThumbnail(_configuration, originalStream, request.AssetThumbnailDto.Extension);
                        BlobProperties thumbnailProps = _azureStorageService.UploadFileToStorage(_configuration, thumbnailStream, string.Concat(version.Key, ".", "png"), out Uri thumbnailUri, true);
                        thumbnail = _azureStorageService.GetBlobSasUri(_configuration, string.Concat(version.Key, ".", "png"), string.Empty, string.Empty, true); //thumbnailUri.AbsoluteUri;
                    }
                    else if (request.AssetThumbnailDto.FileType.ToLower().Contains("nef") || request.AssetThumbnailDto.FileType.ToLower().Contains("raw") || request.AssetThumbnailDto.FileType.ToLower().Contains("heic") || request.AssetThumbnailDto.FileType.ToLower().Contains("dng"))
                    {
                        thumbnail = request.AssetThumbnailDto.Extension;
                    }
                    else
                    {
                        BlobProperties thumbnailProps = _azureStorageService.UploadFileToStorage(_configuration, originalStream, string.Concat(version.Key, ".", request.AssetThumbnailDto.Extension), out Uri thumbnailUri, true);
                        thumbnail = _azureStorageService.GetBlobSasUri(_configuration, string.Concat(version.Key, ".", request.AssetThumbnailDto.Extension), string.Empty, string.Empty, true); //thumbnailUri.AbsoluteUri;
                    }
                }
                else
                {
                    thumbnail = request.AssetThumbnailDto.Extension;
                }
            }
            version.ModifiedDate = DateTimeOffset.UtcNow;
            version.ThumbnailPreview = thumbnail;
            _dbcontext.AssetVersions.Update(version);
            var auditDisplayUser = _dbcontext.AppUsers.First(u => u.Id == request.UserId);

            var auditTrailEntry = new AssetAudit()
            {
                AssetId = Convert.ToInt32(version.AssetId),
                AssetFileName = version.FileName,
                AuditType = Convert.ToInt32(AssetAuditType.AddAssetPreview),
                AuditTypeText = AssetAuditType.AddAssetPreview.GetDescription(),
                AuditCreatedByUserId = version.ModifiedById,
                AuditCreatedDate = DateTimeOffset.UtcNow,
                AuditCreatedByName = auditDisplayUser != null ? auditDisplayUser.UserName : "",
                NewParameters = "Upload new thumbnail"
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
