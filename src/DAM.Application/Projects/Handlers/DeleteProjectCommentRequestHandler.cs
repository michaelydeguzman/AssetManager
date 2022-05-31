using System;
using System.Threading;
using AutoMapper;
using DAM.Persistence;
using System.Linq;
using DAM.Application.Projects.Requests;
using DAM.Application.Projects.Dtos;
using DAM.Domain.Entities;
using System.Collections.Generic;

namespace DAM.Application.Projects.Handlers
{
    public class DeleteProjectCommentRequestHandler : HandlerBase<DeleteProjectCommentRequest, HandlerResult<ProjectCommentDto>>
    {
        private readonly IDbContext _dbcontext;
        private readonly IMapper _mapper;

        public DeleteProjectCommentRequestHandler(IMapper mapper, IDbContext dbcontext)
        {
            _dbcontext = dbcontext ?? throw new ArgumentNullException(nameof(dbcontext));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        public override HandlerResult<ProjectCommentDto> HandleRequest(DeleteProjectCommentRequest request, CancellationToken cancellationToken, HandlerResult<ProjectCommentDto> result)
        {
            var deleteProject = _dbcontext.ProjectComments.FirstOrDefault(x => x.Id == request.ProjectComment.ProjectCommentId);
            deleteProject.Deleted = true;
            deleteProject.ModifiedById = request.UserId;
            deleteProject.ModifiedDate = DateTimeOffset.UtcNow;
            _dbcontext.ProjectComments.Update(deleteProject);

            _dbcontext.SaveChanges();

            result.ResultType = ResultType.Success;
            return result;
        }
    }
}
