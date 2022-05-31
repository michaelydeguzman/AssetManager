using System;
using System.Threading;
using AutoMapper;
using DAM.Application.Carts.Dtos;
using DAM.Application.Carts.Requests;
using DAM.Persistence;
using DAM.Domain.Entities;
using System.Collections.Generic;
using DAM.Application.Assets.Dtos;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using DAM.Application.Projects.Requests;
using DAM.Application.Projects.Dtos;
using DAM.Application.Projects.Enums;
using DAM.Application.Teams.Enums;

namespace DAM.Application.Projects.Handlers
{
    public class ProjectsRequestHandler : HandlerBase<ProjectsRequest, HandlerResult<IEnumerable<ProjectDto>>>
    {
        private readonly IDbContext _dbcontext;
        private readonly IMapper _mapper;

        public ProjectsRequestHandler(IMapper mapper, IDbContext dbcontext)
        {
            _dbcontext = dbcontext ?? throw new ArgumentNullException(nameof(dbcontext));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        public override HandlerResult<IEnumerable<ProjectDto>> HandleRequest(ProjectsRequest request, CancellationToken cancellationToken, HandlerResult<IEnumerable<ProjectDto>> result)
        {
            var allProjects = new List<Project>();

            var isUserAdmin = _dbcontext.AppUsers.Any(x => x.Id == request.UserId && x.UserRoleId == 1) ? true : false;

            if (request.IsArchived)
            {
                allProjects = _dbcontext.Projects.Where(p => p.ProjectStatus == (int)ProjectStatus.Archived && !p.Deleted).ToList();
            } 
            else
            {
                allProjects = _dbcontext.Projects.Where(p => p.ProjectStatus != (int)ProjectStatus.Archived && !p.Deleted).ToList();
            }


            if (isUserAdmin)
            {
                result.Entity = _mapper.Map<IEnumerable<ProjectDto>>(allProjects);
            } 
            else
            {
                var projectIdsFollowedByUser = _dbcontext.ProjectUserFollowers.Where(p => p.UserId == request.UserId).Select(p => p.ProjectId);
                var projectsFollowedByUser = allProjects.Where(p => projectIdsFollowedByUser.Contains(p.Id));
                var projectsCreatedByUser = allProjects.Where(p => p.CreatedById == request.UserId);
                var projectIdsOwnedByUser = _dbcontext.ProjectOwners.Where(p => p.UserId == request.UserId).Select(p => p.ProjectId);
                var projectsOwnedByUser = allProjects.Where(p => projectIdsOwnedByUser.Contains(p.Id));

                var teamIdsWhereUserIsMember = _dbcontext.TeamMembers.Where(m => m.UserId == request.UserId).Select(p => p.TeamId);
                var teamsWhereUserIsMember = _dbcontext.Teams.Where(t => t.Status == (int)TeamStatus.Active && !t.Deleted && teamIdsWhereUserIsMember.Contains(t.Id)).Select(t=>t.Id);
                var projectIdsWhereUserIsTeamMember = _dbcontext.ProjectTeamFollowers.Where(pt => teamsWhereUserIsMember.Contains(pt.TeamId)).Select(p=>p.ProjectId);
                var projectsWhereUserIsTeamMember = allProjects.Where(p => projectIdsWhereUserIsTeamMember.Contains(p.Id));

                var userProjects = projectsOwnedByUser.Union(projectsFollowedByUser).Union(projectsWhereUserIsTeamMember).ToList();

                result.Entity = _mapper.Map<IEnumerable<ProjectDto>>(userProjects);
            }

            foreach (var project in result.Entity)
            {
                var projectItems = _dbcontext.ProjectItems.Where(pi => pi.ProjectId == project.Id.Value).ToList();
                project.ProjectItems = _mapper.Map<List<ProjectItemDto>>(projectItems);
                project.ProjectUploads = _dbcontext.Assets.Where(a => a.ProjectId == project.Id.Value).Select(a => a.Id).ToList();
            }

            result.ResultType = ResultType.Success;
            return result;
        }
    }
}
