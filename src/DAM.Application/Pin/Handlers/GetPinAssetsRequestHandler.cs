using Microsoft.Extensions.Configuration;
using DAM.Application.Pin.Dtos;
using DAM.Persistence;
using System;
using System.Collections.Generic;
using System.Text;
using static DAM.Application.Pin.Requests.PinAssetRequest;
using System.Threading;
using System.Linq;
using AutoMapper;

namespace DAM.Application.Pin.Handlers
{
    public class GetPinAssetsRequestHandler : HandlerBase<GetPinAssetsRequest, HandlerResult<IEnumerable<PinAssetsDto>>>
    {
        private readonly IDbContext _dbcontext;
        private readonly IConfiguration _configuration;
        private readonly IMapper _mapper;

        public GetPinAssetsRequestHandler(IDbContext dbcontext, IConfiguration configuration, IMapper mapper)
        {
            _dbcontext = dbcontext ?? throw new ArgumentNullException(nameof(dbcontext));
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        public override HandlerResult<IEnumerable<PinAssetsDto>> HandleRequest(GetPinAssetsRequest request, CancellationToken cancellationToken, HandlerResult<IEnumerable<PinAssetsDto>> result)
        {
            var pinsList = _dbcontext.PinAssets.Where(a => a.UserId == request.UserId).OrderBy(a => a.OrderNumber);
            result.Entity = _mapper.Map<IEnumerable<PinAssetsDto>>(pinsList);
            if(result.Entity.Any())
            {
                result.ResultType = ResultType.Success;
            }
            else
            {
                result.ResultType = ResultType.NoData;
            }
            return result;
        }
    }
}
