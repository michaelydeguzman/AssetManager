using Microsoft.Extensions.Configuration;
using DAM.Application.Pin.Dtos;
using DAM.Persistence;
using System;
using System.Collections.Generic;
using System.Text;
using static DAM.Application.Pin.Requests.PinFolderRequest;
using System.Threading;
using System.Linq;
using AutoMapper;
using DAM.Domain.Entities;

namespace DAM.Application.Pin.Handlers
{
    public class OrderPinFoldersRequestHandler : HandlerBase<OrderPinFoldersRequest, HandlerResult<IEnumerable<PinFoldersDto>>>
    {
        private readonly IDbContext _dbcontext;
        private readonly IConfiguration _configuration;
        private readonly IMapper _mapper;

        public OrderPinFoldersRequestHandler(IDbContext dbcontext, IConfiguration configuration, IMapper mapper)
        {
            _dbcontext = dbcontext ?? throw new ArgumentNullException(nameof(dbcontext));
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        public override HandlerResult<IEnumerable<PinFoldersDto>> HandleRequest(OrderPinFoldersRequest request, CancellationToken cancellationToken, HandlerResult<IEnumerable<PinFoldersDto>> result)
        {
            if(request.PinObj.OrderedFolderIds.Count > 0)
            {
                var newOrderedPinList = new List<PinFolder>();
                request.PinObj.OrderedFolderIds.ForEach(p =>
                {
                    var pin = new PinFolder
                    {
                        UserId = request.PinObj.UserId,
                        FolderId = p,
                        OrderNumber = request.PinObj.OrderedFolderIds.IndexOf(p) + 1
                    };
                    newOrderedPinList.Add(pin);
                });
                var prePinList = _dbcontext.PinFolders.Where(a => a.UserId == request.PinObj.UserId);
                _dbcontext.PinFolders.RemoveRange(prePinList);
                _dbcontext.PinFolders.AddRange(newOrderedPinList);
                _dbcontext.SaveChanges();
            }
            result.ResultType = ResultType.Success;
            var latestPinFolderList = _dbcontext.PinFolders.Where(a => a.UserId == request.PinObj.UserId).OrderBy(a => a.OrderNumber);
            result.Entity = _mapper.Map<IEnumerable<PinFoldersDto>>(latestPinFolderList);
            return result;
        }
    }
}
