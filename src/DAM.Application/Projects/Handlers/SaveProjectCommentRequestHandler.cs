using System;
using System.Threading;
using AutoMapper;
using DAM.Persistence;
using System.Linq;
using DAM.Application.Projects.Requests;
using DAM.Application.Projects.Dtos;
using DAM.Domain.Entities;
using System.Collections.Generic;
using DAM.Application.Services.Interfaces;

namespace DAM.Application.Projects.Handlers
{
    public class SaveProjectCommentRequestHandler : HandlerBase<SaveProjectCommentRequest, HandlerResult<ProjectCommentDto>>
    {
        private readonly IDbContext _dbcontext;
        private readonly IMapper _mapper;
        private readonly IEmailService _emailService;

        public SaveProjectCommentRequestHandler(IMapper mapper, IDbContext dbcontext, IEmailService emailService)
        {
            _dbcontext = dbcontext ?? throw new ArgumentNullException(nameof(dbcontext));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _emailService = emailService ?? throw new ArgumentNullException(nameof(emailService));
        }

        public override HandlerResult<ProjectCommentDto> HandleRequest(SaveProjectCommentRequest request, CancellationToken cancellationToken, HandlerResult<ProjectCommentDto> result)
        {
            
            if (request.ProjectComment.Id.HasValue)
            {
                var updateProject = _dbcontext.ProjectComments.FirstOrDefault(x => x.Id == request.ProjectComment.Id.Value);
                updateProject.Comment = request.ProjectComment.Comment;
                updateProject.ModifiedById = request.UserId;
                updateProject.ModifiedDate = DateTimeOffset.UtcNow;
                _dbcontext.ProjectComments.Update(updateProject);
            } 
            else
            {
                var newProjectComment = _mapper.Map<ProjectComment>(request.ProjectComment);
                newProjectComment.CreatedById = request.UserId;
                _dbcontext.ProjectComments.Add(newProjectComment);
               
            }
            _dbcontext.SaveChanges();

            if (request.ProjectComment.Mentions.Count > 0)
            {
                _emailService.SaveProjectCommentToEmailQueue(request.ProjectComment.Mentions, request.UserId, request.ProjectComment.ProjectId, request.ProjectComment.Comment);

            }

            result.ResultType = ResultType.Success;
            return result;
        }
    }
}
