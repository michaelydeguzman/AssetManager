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
using DAM.Domain.Entities;

namespace DAM.Application.Pin.Handlers
{
    public class OrderPinAssetsRequestHandler : HandlerBase<OrderPinAssetsRequest, HandlerResult<IEnumerable<PinAssetsDto>>>
    {
        private readonly IDbContext _dbcontext;
        private readonly IConfiguration _configuration;
        private readonly IMapper _mapper;

        public OrderPinAssetsRequestHandler(IDbContext dbcontext, IConfiguration configuration, IMapper mapper)
        {
            _dbcontext = dbcontext ?? throw new ArgumentNullException(nameof(dbcontext));
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        public override HandlerResult<IEnumerable<PinAssetsDto>> HandleRequest(OrderPinAssetsRequest request, CancellationToken cancellationToken, HandlerResult<IEnumerable<PinAssetsDto>> result)
        {
            if(request.PinObj.OrderedAssetIds.Count > 0)
            {
                var newOrderedPinList = new List<PinAsset>();
                request.PinObj.OrderedAssetIds.ForEach(p =>
                {
                    var pin = new PinAsset
                    {
                        UserId = request.PinObj.UserId,
                        AssetId = p,
                        OrderNumber = request.PinObj.OrderedAssetIds.IndexOf(p) + 1
                    };
                    newOrderedPinList.Add(pin);
                });
                var prePinList = _dbcontext.PinAssets.Where(a => a.UserId == request.PinObj.UserId);
                _dbcontext.PinAssets.RemoveRange(prePinList);
                _dbcontext.PinAssets.AddRange(newOrderedPinList);
                _dbcontext.SaveChanges();
            }
            result.ResultType = ResultType.Success;
            var latestPinAssetList = _dbcontext.PinAssets.Where(a => a.UserId == request.PinObj.UserId).OrderBy(a => a.OrderNumber);
            result.Entity = _mapper.Map<IEnumerable<PinAssetsDto>>(latestPinAssetList);
            return result;
        }
    }
}
