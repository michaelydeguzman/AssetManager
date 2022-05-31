using AutoMapper;
using AutoMapper.Configuration;
using DAM.Application.Cache;
using DAM.Application.FeatureFlags.Dtos;
using DAM.Application.FeatureFlags.Requests;
using DAM.Application.UserRoles.Dtos;
using DAM.Domain.Entities;
using DAM.Persistence;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using static DAM.Application.UserRoles.Requests.UserRolesRequest;

namespace DAM.Application.UserRoles.Handlers
{
    public class GetFeatureFlagsRequestHandler : HandlerBase<GetFeatureFlagsRequest, HandlerResult<IEnumerable<FeatureFlagDto>>>
    {
        private readonly ICacheProvider _cacheProvider;
        private readonly IDbContext _dbcontext;
        private readonly IMapper _mapper;

        public GetFeatureFlagsRequestHandler(ICacheProvider cacheProvider, IDbContext dbcontext, IMapper mapper)
        {
            _cacheProvider = cacheProvider ?? throw new ArgumentNullException(nameof(cacheProvider));
            _dbcontext = dbcontext ?? throw new ArgumentNullException(nameof(dbcontext));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        public override HandlerResult<IEnumerable<FeatureFlagDto>> HandleRequest(GetFeatureFlagsRequest request, CancellationToken cancellationToken, HandlerResult<IEnumerable<FeatureFlagDto>> result)
        {
            result.Entity = _mapper.Map<IEnumerable<FeatureFlagDto>>(_dbcontext.FeatureFlags);
            result.ResultType = ResultType.Success;
            return result;
        }

        public override Exception OnException(Exception ex)
        {
            return new Exception(ex.Message, ex);
        }
    }
}
