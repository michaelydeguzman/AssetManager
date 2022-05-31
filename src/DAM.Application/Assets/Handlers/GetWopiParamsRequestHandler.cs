using AutoMapper;
using DAM.Application.Assets.Dtos;
using DAM.Application.Assets.Requests;
using DAM.Application.Cache;
using DAM.Persistence;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Microsoft.Extensions.Configuration;
using DAM.Application.Assets.Helpers;
using Microsoft.Extensions.Caching.Memory;

namespace DAM.Application.Assets.Handlers
{
    public class GetWopiParamsRequestHandler : HandlerBase<GetWopiParamsRequest, HandlerResult<WopiParamDto>>
    {
        private readonly ICacheProvider _cacheProvider;
        private readonly IDbContext _dbcontext;
        private readonly IMapper _mapper;
        private readonly IConfiguration _configuration;
        private readonly IMemoryCache _memoryCache;

        public GetWopiParamsRequestHandler(ICacheProvider cacheProvider, IDbContext dbcontext, IMapper mapper, IConfiguration configuration, IMemoryCache memoryCache)
        {
            _cacheProvider = cacheProvider ?? throw new ArgumentNullException(nameof(cacheProvider));
            _dbcontext = dbcontext ?? throw new ArgumentNullException(nameof(dbcontext));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
            _memoryCache = memoryCache ?? throw new ArgumentNullException(nameof(memoryCache));
        }

        public override HandlerResult<WopiParamDto> HandleRequest(GetWopiParamsRequest request, CancellationToken cancellationToken, HandlerResult<WopiParamDto> result)
        {
            WopiParamDto wopiParamDto = new WopiParamDto();

            var asset = _dbcontext.AssetVersions.FirstOrDefault(x => x.Key == request.AssetKey);
            var actionStr = "";

            switch (request.Action)
            {
                case 1:
                    actionStr = "view";
                    break;
                case 2:
                    actionStr = "edit";
                    break;
            }

            if (asset != null)
            {
                List<WopiActionDto> discoData = WopiUtil.GetDiscoveryInfo(_configuration, _memoryCache).GetAwaiter().GetResult();

                var fileExt = asset.Extension;
                var action = discoData.FirstOrDefault(i => i.name == actionStr && i.ext == fileExt);

                // Make sure the action isn't null
                if (action != null)
                {
                    string urlsrc = WopiUtil.GetActionUrl(_configuration, action, asset);

                    wopiParamDto.AccessToken = string.Empty; // generate access token
                    wopiParamDto.AccessTokenTTL = 0; // access token ttl
                    wopiParamDto.WopiUrlSrc = urlsrc;

                    result.Entity = wopiParamDto;
                }
            }
            return result;
        }

        public override Exception OnException(Exception ex)
        {
            return new Exception(ex.Message, ex);
        }

    }
}