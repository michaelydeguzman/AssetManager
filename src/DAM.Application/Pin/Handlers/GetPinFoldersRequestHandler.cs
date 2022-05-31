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

namespace DAM.Application.Pin.Handlers
{
    public class GetPinFoldersRequestHandler : HandlerBase<GetPinFoldersRequest, HandlerResult<IEnumerable<PinFoldersDto>>>
    {
        private readonly IDbContext _dbcontext;
        private readonly IConfiguration _configuration;
        private readonly IMapper _mapper;

        public GetPinFoldersRequestHandler(IDbContext dbcontext, IConfiguration configuration, IMapper mapper)
        {
            _dbcontext = dbcontext ?? throw new ArgumentNullException(nameof(dbcontext));
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        public override HandlerResult<IEnumerable<PinFoldersDto>> HandleRequest(GetPinFoldersRequest request, CancellationToken cancellationToken, HandlerResult<IEnumerable<PinFoldersDto>> result)
        {
            var allFolders = _dbcontext.Folders.ToList();;
            var pinsList = _dbcontext.PinFolders.Where(a => a.UserId == request.UserId).OrderBy(a => a.OrderNumber);
            result.Entity = _mapper.Map<IEnumerable<PinFoldersDto>>(pinsList);
            foreach (var folder in result.Entity)
            {
                var delta = allFolders.FirstOrDefault(f => f.Id == folder.FolderId);
                folder.FolderName = delta.FolderName;
            }
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
