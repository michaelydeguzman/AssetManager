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
    public class ProjectCommentsRequestHandler : HandlerBase<ProjectCommentsRequest, HandlerResult<IEnumerable<ProjectCommentDto>>>
    {
        private readonly IDbContext _dbcontext;
        private readonly IMapper _mapper;

        public ProjectCommentsRequestHandler(IMapper mapper, IDbContext dbcontext)
        {
            _dbcontext = dbcontext ?? throw new ArgumentNullException(nameof(dbcontext));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        public override HandlerResult<IEnumerable<ProjectCommentDto>> HandleRequest(ProjectCommentsRequest request, CancellationToken cancellationToken, HandlerResult<IEnumerable<ProjectCommentDto>> result)
        {
            var projectComments = _dbcontext.ProjectComments.Where(p => p.ProjectId == request.ProjectId && !p.Deleted).OrderByDescending(p=>p.CreatedDate);

            result.Entity = _mapper.Map<List<ProjectCommentDto>>(projectComments);

            result.ResultType = ResultType.Success;
            return result;
        }
    }
}
