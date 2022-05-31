using AutoMapper;

using DAM.Application.Assets.Dtos;

using DAM.Application.Cache;

using DAM.Persistence;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Microsoft.Extensions.Configuration;
using DAM.Application.Assets.Enums;
using DAM.Application.Services.Interfaces;
using DAM.Application.Extensions;
using System.IO;
using DAM.Application.Watermarks.Requests;
using DAM.Application.Watermarks.Dto;
using DAM.Domain.Entities;

namespace DAM.Application.Watermarks.Handlers
{
    public class SaveDefaultWatermarkRequestHandler : HandlerBase<SaveDefaultWatermarkRequest, HandlerResult<WatermarkDto>>
    {
        private readonly IDbContext _dbcontext;
        private readonly IMapper _mapper;
        private readonly IConfiguration _configuration;
        private readonly IAzureStorageService _azureStorageService;

        public SaveDefaultWatermarkRequestHandler(ICacheProvider cacheProvider, IDbContext dbcontext, IMapper mapper, IConfiguration configuration, IAzureStorageService azureStorageService)
        {
            _dbcontext = dbcontext ?? throw new ArgumentNullException(nameof(dbcontext));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
            _azureStorageService = azureStorageService ?? throw new ArgumentNullException(nameof(azureStorageService));
        }

        public override HandlerResult<WatermarkDto> HandleRequest(SaveDefaultWatermarkRequest request, CancellationToken cancellationToken, HandlerResult<WatermarkDto> result)
        {
            result.Entity = new WatermarkDto();
            var defaultWatermark = new Watermark();

            bool IsEdit = false;
            bool saveNewImage = request.DefaultWatermark.Watermark != null;

            if (request.DefaultWatermark.Id.HasValue)
            {
                defaultWatermark = _dbcontext.Watermarks.FirstOrDefault(x => x.Id == request.DefaultWatermark.Id.Value);

                if (defaultWatermark == null)
                {
                    result.ResultType = ResultType.NoData;
                    return result;
                }
                else
                {
                    defaultWatermark.Opacity = request.DefaultWatermark.Opacity;
                    defaultWatermark.Size = request.DefaultWatermark.Size;
                    defaultWatermark.WatermarkPosition = request.DefaultWatermark.WatermarkPosition;
                    defaultWatermark.ModifiedById = request.UserId;
                    defaultWatermark.ModifiedDate = DateTimeOffset.UtcNow;
                    IsEdit = true;
                }
            } 
            else
            {
                //save new default watermark
                defaultWatermark = _mapper.Map<Watermark>(request.DefaultWatermark);
                defaultWatermark.CreatedById = request.UserId;
            }

            if (saveNewImage)
            {
                // Save to blob
                using (Stream wmStream = new MemoryStream(request.DefaultWatermark.Watermark))
                {
                    var fileName = "defaultWatermark.png";
                    _azureStorageService.SaveDefaultWatermark(_configuration, wmStream, fileName, out Uri blobUri);
                    defaultWatermark.WatermarkUrl = _azureStorageService.GetWatermarkUrl(_configuration, fileName);
                }
            }

            if (IsEdit)
            {
                _dbcontext.Watermarks.Update(defaultWatermark);
            }
            else
            {
                _dbcontext.Watermarks.Add(defaultWatermark);
            }
          
            _dbcontext.SaveChanges();
            result.Entity = _mapper.Map<WatermarkDto>(defaultWatermark);
            result.ResultType = ResultType.Success;
            
            return result;
        }

        public override Exception OnException(Exception ex)
        {
            return new Exception(ex.Message, ex);
        }

    }
}