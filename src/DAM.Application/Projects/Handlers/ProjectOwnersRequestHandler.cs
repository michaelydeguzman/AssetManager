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
    public class ProjectOwnersRequestHandler : HandlerBase<ProjectOwnersRequest, HandlerResult<IEnumerable<ProjectOwnerDto>>>
    {
        private readonly IDbContext _dbcontext;
        private readonly IMapper _mapper;

        public ProjectOwnersRequestHandler(IMapper mapper, IDbContext dbcontext)
        {
            _dbcontext = dbcontext ?? throw new ArgumentNullException(nameof(dbcontext));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        public override HandlerResult<IEnumerable<ProjectOwnerDto>> HandleRequest(ProjectOwnersRequest request, CancellationToken cancellationToken,
            HandlerResult<IEnumerable<ProjectOwnerDto>> result)
        {
            var projectOwners = _dbcontext.ProjectOwners.Where(p => p.ProjectId == request.ProjectId);

            result.Entity = _mapper.Map<List<ProjectOwnerDto>>(projectOwners);
          
            result.ResultType = ResultType.Success;
            return result;
        }
    }
}
