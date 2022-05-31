using System;
using System.Threading;
using AutoMapper;
using DAM.Persistence;
using System.Linq;
using DAM.Application.Projects.Requests;
using DAM.Application.Projects.Dtos;
using DAM.Domain.Entities;
using System.Collections.Generic;
using DAM.Application.Projects.Enums;
using DAM.Application.Services.Interfaces;

namespace DAM.Application.Projects.Handlers
{
    public class DeleteProjectRequestHandler : HandlerBase<DeleteProjectRequest, HandlerResult<ProjectDto>>
    {
        private readonly IDbContext _dbcontext;
        private readonly IMapper _mapper;
        private readonly IEmailService _emailService;

        public DeleteProjectRequestHandler(IMapper mapper, IDbContext dbcontext, IEmailService emailService)
        {
            _dbcontext = dbcontext ?? throw new ArgumentNullException(nameof(dbcontext));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _emailService = emailService ?? throw new ArgumentNullException(nameof(emailService));
        }

        public override HandlerResult<ProjectDto> HandleRequest(DeleteProjectRequest request, CancellationToken cancellationToken, HandlerResult<ProjectDto> result)
        {
            var updateProject = _dbcontext.Projects.FirstOrDefault(p => p.Id == request.ProjectId);

            updateProject.Deleted = true;
            updateProject.ModifiedDate = DateTimeOffset.UtcNow;
            updateProject.ModifiedById = request.UserId;

            _dbcontext.Projects.Update(updateProject);
            _dbcontext.SaveChanges();

            // send project deleted email to owners and collaborators
           
            var projectOwners = _dbcontext.ProjectOwners.Where(p => p.ProjectId == request.ProjectId).Select(po=>po.UserId).ToList();
            var projectFollowers = _dbcontext.ProjectUserFollowers.Where(p => p.ProjectId == request.ProjectId).Select(pu => pu.UserId).ToList();

            _emailService.SaveProjectDeletedOwnerToEmailQueue(projectOwners, request.ProjectId, updateProject.ProjectName, request.UserId);
            _emailService.SaveProjectDeletedCollaboratorToEmailQueue(projectFollowers, request.ProjectId, updateProject.ProjectName, request.UserId);

            result.Entity = _mapper.Map<ProjectDto>(updateProject);

            _dbcontext.SaveChanges();

            result.ResultType = ResultType.Success;
            return result;
        }
    }
}
