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
    public class ConvertPDFToThumbnailImageRequestHandler : HandlerBase<ConvertPDFToThumbnailImageRequest, HandlerResult<DownloadDto>>
    {
        private readonly ICacheProvider _cacheProvider;
        private readonly IDbContext _dbcontext;
        private readonly IMapper _mapper;
        private readonly IConfiguration _configuration;
        private readonly IAzureStorageService _azureStorageService;
        private readonly IConversionService _conversionService;
        private readonly ITagService _tagService;
        private readonly IHelperService _helperService;

        public ConvertPDFToThumbnailImageRequestHandler(ICacheProvider cacheProvider, IDbContext dbcontext, IMapper mapper, IConfiguration configuration, IAzureStorageService azureStorageService,
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

        public override HandlerResult<DownloadDto> HandleRequest(ConvertPDFToThumbnailImageRequest request, CancellationToken cancellationToken, HandlerResult<DownloadDto> result)
        {
            BlobProperties ap = new BlobProperties();
            Stream thumbnailStream;

            result.Entity = new DownloadDto()
            {
                Content = new MemoryStream(),
                FileName = "",
                ContentType = ""
            };

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
                }
                else
                {
                    //convert fail;
                }
            }

            thumbnailStream.Seek(0, SeekOrigin.Begin);
            thumbnailStream.CopyTo(result.Entity.Content);
            result.Entity.Content.Seek(0, SeekOrigin.Begin);
            result.Entity.ContentType = "image/jpeg";
            result.Entity.FileName = "convertedImage";
            result.ResultType = ResultType.Success;

            return result;
        }

        public override Exception OnException(Exception ex)
        {
            return new Exception(ex.Message, ex);
        }
    }
}
