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
    public class ReplacePinFoldersRequestHandler : HandlerBase<RemovePinFoldersRequest, HandlerResult<IEnumerable<PinFoldersDto>>>
    {
        private readonly IDbContext _dbcontext;
        private readonly IConfiguration _configuration;
        private readonly IMapper _mapper;

        public ReplacePinFoldersRequestHandler(IDbContext dbcontext, IConfiguration configuration, IMapper mapper)
        {
            _dbcontext = dbcontext ?? throw new ArgumentNullException(nameof(dbcontext));
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        public override HandlerResult<IEnumerable<PinFoldersDto>> HandleRequest(RemovePinFoldersRequest request, CancellationToken cancellationToken, HandlerResult<IEnumerable<PinFoldersDto>> result)
        {
            if (request.PinObj.FolderId > 0)
            {
                var prePinList = _dbcontext.PinFolders.Where(a => a.UserId == request.PinObj.UserId).OrderBy(a => a.OrderNumber).ToList();
                _dbcontext.PinFolders.RemoveRange(prePinList);

                prePinList.Remove(prePinList.Where(a => a.FolderId == request.PinObj.FolderId).FirstOrDefault());
                var newPinList = new List<PinFolder>();
                prePinList.ForEach(p =>
                {
                    var pin = new PinFolder
                    {
                        FolderId = p.FolderId,
                        UserId = p.UserId,
                        OrderNumber = prePinList.IndexOf(p) + 1
                    };
                    newPinList.Add(pin);
                });
                _dbcontext.PinFolders.AddRange(newPinList);
                _dbcontext.SaveChanges();
            }
            var latestPinFolderList = _dbcontext.PinFolders.Where(a => a.UserId == request.PinObj.UserId).OrderBy(a => a.OrderNumber);
            result.Entity = _mapper.Map<IEnumerable<PinFoldersDto>>(latestPinFolderList);
            result.ResultType = ResultType.Success;
            return result;
        }
    }
}
