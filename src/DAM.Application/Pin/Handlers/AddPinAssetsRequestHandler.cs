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
using DAM.Application.Assets.Enums;

namespace DAM.Application.Pin.Handlers
{
    public class AddPinAssetsRequestHandler : HandlerBase<AddPinAssetsRequest, HandlerResult<IEnumerable<PinAssetsDto>>>
    {
        private readonly IDbContext _dbcontext;
        private readonly IConfiguration _configuration;
        private readonly IMapper _mapper;

        public AddPinAssetsRequestHandler(IDbContext dbcontext, IConfiguration configuration, IMapper mapper)
        {
            _dbcontext = dbcontext ?? throw new ArgumentNullException(nameof(dbcontext));
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        public override HandlerResult<IEnumerable<PinAssetsDto>> HandleRequest(AddPinAssetsRequest request, CancellationToken cancellationToken, HandlerResult<IEnumerable<PinAssetsDto>> result)
        {
            //do clean the pin asset which is archive or delete           
            var oldPinList = _dbcontext.PinAssets.Where(a => a.UserId == request.PinObj.UserId).ToList();
            if( oldPinList.Count > 0)
            {
                var pinIds = new List<int>();
                oldPinList.ForEach(p =>
                {
                    pinIds.Add(p.AssetId);
                });
                var assetList = _dbcontext.Assets.Where(a => pinIds.Contains(a.Id) && (a.Status == Convert.ToInt32(AssetStatus.Archived) || a.Status == Convert.ToInt32(AssetStatus.Deleted))).ToList();
                if(assetList.Count > 0)
                {
                    var removeList = new List<PinAsset>();
                    assetList.ForEach(a =>
                    {
                        var pin = oldPinList.Find(p => p.AssetId == a.Id);
                        removeList.Add(pin);
                    });
                    _dbcontext.PinAssets.RemoveRange(removeList);
                    _dbcontext.SaveChanges();
                }
            }

            //add new pin
            if (request.PinObj.AssetId > 0)
            { 
                var prePinAssetList = _dbcontext.PinAssets.Where(a => a.UserId == request.PinObj.UserId).ToList();
                if(prePinAssetList.Count >= 12)
                {
                    var fullPinList = _dbcontext.PinAssets.Where(a => a.UserId == request.PinObj.UserId);
                    result.Entity = _mapper.Map<IEnumerable<PinAssetsDto>>(fullPinList);
                    result.ResultType = ResultType.Fail;
                    return result;
                }

                if (!prePinAssetList.Exists(p => p.AssetId == request.PinObj.AssetId))
                {
                    _dbcontext.PinAssets.RemoveRange(prePinAssetList);
                    var newPinAssetList = new List<PinAsset>();
                    prePinAssetList.ForEach((p) =>
                    {
                        var pin = new PinAsset
                        {
                            UserId = p.UserId,
                            AssetId = p.AssetId,
                            OrderNumber = p.OrderNumber
                        };
                        newPinAssetList.Add(pin);
                    });
                    var newPin = new PinAsset
                    {
                        UserId = request.PinObj.UserId,
                        AssetId = request.PinObj.AssetId,
                        OrderNumber = prePinAssetList.Count + 1
                    };
                    newPinAssetList.Add(newPin);
                    _dbcontext.PinAssets.AddRange(newPinAssetList);
                    _dbcontext.SaveChanges();
                }
            }
            var latestPinAssetList = _dbcontext.PinAssets.Where(a => a.UserId == request.PinObj.UserId).OrderBy(a => a.OrderNumber);
            result.Entity = _mapper.Map<IEnumerable<PinAssetsDto>>(latestPinAssetList);
            result.ResultType = ResultType.Success;
            return result;
        }
    }
}
