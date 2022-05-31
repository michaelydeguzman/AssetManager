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
    public class ReplacePinAssetsRequestHandler : HandlerBase<RemovePinAssetsRequest, HandlerResult<IEnumerable<PinAssetsDto>>>
    {
        private readonly IDbContext _dbcontext;
        private readonly IConfiguration _configuration;
        private readonly IMapper _mapper;

        public ReplacePinAssetsRequestHandler(IDbContext dbcontext, IConfiguration configuration, IMapper mapper)
        {
            _dbcontext = dbcontext ?? throw new ArgumentNullException(nameof(dbcontext));
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        public override HandlerResult<IEnumerable<PinAssetsDto>> HandleRequest(RemovePinAssetsRequest request, CancellationToken cancellationToken, HandlerResult<IEnumerable<PinAssetsDto>> result)
        {
            if (request.PinObj.AssetId > 0)
            {
                var prePinList = _dbcontext.PinAssets.Where(a => a.UserId == request.PinObj.UserId).OrderBy(a => a.OrderNumber).ToList();
                _dbcontext.PinAssets.RemoveRange(prePinList);

                prePinList.Remove(prePinList.Where(a => a.AssetId == request.PinObj.AssetId).FirstOrDefault());
                var newPinList = new List<PinAsset>();
                prePinList.ForEach(p =>
                {
                    var pin = new PinAsset
                    {
                        AssetId = p.AssetId,
                        UserId = p.UserId,
                        OrderNumber = prePinList.IndexOf(p) + 1
                    };
                    newPinList.Add(pin);
                });
                _dbcontext.PinAssets.AddRange(newPinList);
                _dbcontext.SaveChanges();
            }
            var latestPinAssetList = _dbcontext.PinAssets.Where(a => a.UserId == request.PinObj.UserId).OrderBy(a => a.OrderNumber);
            result.Entity = _mapper.Map<IEnumerable<PinAssetsDto>>(latestPinAssetList);
            result.ResultType = ResultType.Success;
            return result;
        }
    }
}
