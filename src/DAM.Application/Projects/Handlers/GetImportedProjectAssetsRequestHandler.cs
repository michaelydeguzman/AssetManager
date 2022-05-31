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
    public class GetImportedProjectAssetsRequestHandler : HandlerBase<GetImportedProjectAssetsRequest, HandlerResult<IEnumerable<ProjectItemDto>>>
    {
        private readonly IDbContext _dbcontext;
        private readonly IMapper _mapper;

        public GetImportedProjectAssetsRequestHandler(IMapper mapper, IDbContext dbcontext)
        {
            _dbcontext = dbcontext ?? throw new ArgumentNullException(nameof(dbcontext));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        public override HandlerResult<IEnumerable<ProjectItemDto>> HandleRequest(GetImportedProjectAssetsRequest request, CancellationToken cancellationToken, HandlerResult<IEnumerable<ProjectItemDto>> result)
        {
            var projectItems = _dbcontext.ProjectItems.Where(pi => pi.ProjectId == request.ProjectId);

            result.Entity = _mapper.Map<IEnumerable<ProjectItemDto>>(projectItems);

            result.ResultType = ResultType.Success;
            return result;
        }
    }
}
