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

namespace DAM.Application.Watermarks.Handlers
{
    public class GetDefaultWatermarkRequestHandler : HandlerBase<GetDefaultWatermarkRequest, HandlerResult<WatermarkDto>>
    {
        private readonly IDbContext _dbcontext;
        private readonly IMapper _mapper;
        private readonly IConfiguration _configuration;

        public GetDefaultWatermarkRequestHandler(ICacheProvider cacheProvider, IDbContext dbcontext, IMapper mapper, IConfiguration configuration, IAzureStorageService azureStorageService)
        {
            _dbcontext = dbcontext ?? throw new ArgumentNullException(nameof(dbcontext));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
        }

        public override HandlerResult<WatermarkDto> HandleRequest(GetDefaultWatermarkRequest request, CancellationToken cancellationToken, HandlerResult<WatermarkDto> result)
        {
            result.Entity = new WatermarkDto();
            var defaultWatermark = _dbcontext.Watermarks.FirstOrDefault(x => !x.CompanyId.HasValue && !x.FolderId.HasValue);

            if (defaultWatermark != null)
            {
                result.Entity = _mapper.Map<WatermarkDto>(defaultWatermark);
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