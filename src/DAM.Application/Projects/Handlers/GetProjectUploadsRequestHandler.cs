using System;
using System.Threading;
using AutoMapper;
using DAM.Persistence;
using System.Collections.Generic;
using System.Linq;
using DAM.Application.Projects.Requests;
using DAM.Application.Projects.Dtos;

namespace DAM.Application.Projects.Handlers
{
    public class GetProjectUploadsRequestHandler : HandlerBase<GetProjectUploadsRequest, HandlerResult<IEnumerable<int>>>
    {
        private readonly IDbContext _dbcontext;
        private readonly IMapper _mapper;

        public GetProjectUploadsRequestHandler(IMapper mapper, IDbContext dbcontext)
        {
            _dbcontext = dbcontext ?? throw new ArgumentNullException(nameof(dbcontext));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        public override HandlerResult<IEnumerable<int>> HandleRequest(GetProjectUploadsRequest request, CancellationToken cancellationToken, HandlerResult<IEnumerable<int>> result)
        {
            var assetIds = _dbcontext.Assets.Where(a => a.ProjectId == request.ProjectId).Select(a=>a.Id).ToList();

            result.Entity = assetIds;

            result.ResultType = ResultType.Success;
            return result;
        }
    }
}
