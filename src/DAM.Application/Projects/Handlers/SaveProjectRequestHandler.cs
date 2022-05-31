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
using DAM.Application.Projects.Enums;
using DAM.Application.Services;

namespace DAM.Application.Projects.Handlers
{
    public class SaveProjectRequestHandler : HandlerBase<SaveProjectRequest, HandlerResult<ProjectDto>>
    {
        private readonly IDbContext _dbcontext;
        private readonly IMapper _mapper;
        private readonly IEmailService _emailService;

        public SaveProjectRequestHandler(IMapper mapper, IDbContext dbcontext, IEmailService emailService)
        {
            _dbcontext = dbcontext ?? throw new ArgumentNullException(nameof(dbcontext));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _emailService = emailService ?? throw new ArgumentNullException(nameof(emailService));
        }

        public override HandlerResult<ProjectDto> HandleRequest(SaveProjectRequest request, CancellationToken cancellationToken, HandlerResult<ProjectDto> result)
        {
            var projectId = 0;
            var projectUpdateDetails = new List<ProjectUpdateDetailsDto>();

            if (request.Project.Id.HasValue)
            {
                projectId = request.Project.Id.Value;
                var updateProject = _dbcontext.Projects.FirstOrDefault(p => p.Id == projectId);

                if (updateProject.ProjectName != request.Project.ProjectName)
                {
                    projectUpdateDetails.Add(
                        new ProjectUpdateDetailsDto()
                        {
                            ProjectId = projectId,
                            FieldName = "Project Name",
                            OldValue = updateProject.ProjectName,
                            NewValue = request.Project.ProjectName
                        }
                    );

                    updateProject.ProjectName = request.Project.ProjectName;
                };

                if (updateProject.ProjectOverview != request.Project.ProjectOverview)
                {
                    projectUpdateDetails.Add(
                        new ProjectUpdateDetailsDto()
                        {
                            ProjectId = projectId,
                            FieldName = "Project Overview",
                            OldValue = updateProject.ProjectOverview,
                            NewValue = request.Project.ProjectOverview
                        }
                    );

                    updateProject.ProjectOverview = request.Project.ProjectOverview;
                };

                if (updateProject.ProjectDueDate != request.Project.ProjectDueDate)
                {
                    projectUpdateDetails.Add(
                        new ProjectUpdateDetailsDto()
                        {
                            ProjectId = projectId,
                            FieldName = "Due Date",
                            OldValue = updateProject.ProjectDueDate.HasValue ? updateProject.ProjectDueDate.ToString() : "",
                            NewValue = request.Project.ProjectDueDate.HasValue ? request.Project.ProjectDueDate.ToString() : ""
                        }
                    );

                    updateProject.ProjectDueDate = request.Project.ProjectDueDate;
                };

                if (updateProject.ProjectStatus != request.Project.ProjectStatus)
                {
                    projectUpdateDetails.Add(
                        new ProjectUpdateDetailsDto()
                        {
                            ProjectId = projectId,
                            FieldName = "Status",
                            OldValue = ((ProjectStatus)updateProject.ProjectStatus).GetEnumDescription(),
                            NewValue = ((ProjectStatus)request.Project.ProjectStatus).GetEnumDescription()
                        }
                    );

                    updateProject.ProjectStatus = request.Project.ProjectStatus;
                };

                updateProject.ModifiedById = request.UserId;
                updateProject.ModifiedDate = DateTimeOffset.UtcNow;

                _dbcontext.Projects.Update(updateProject);
                _dbcontext.SaveChanges();

                result.Entity = _mapper.Map<ProjectDto>(updateProject);
            }
            else
            {
                var newProject = new Project()
                {
                    ProjectName = request.Project.ProjectName,
                    ProjectOverview = request.Project.ProjectOverview,
                    ProjectDueDate = request.Project.ProjectDueDate,
                    ProjectStatus = request.Project.ProjectStatus,
                    CreatedById = request.UserId
                };

                _dbcontext.Projects.Add(newProject);
                _dbcontext.SaveChanges();

                projectId = newProject.Id;
                result.Entity = _mapper.Map<ProjectDto>(newProject);
            }

            var currentProjectOwners = _dbcontext.ProjectOwners.Where(puf => puf.ProjectId == projectId).ToList();
            var currentProjectUsers = _dbcontext.ProjectUserFollowers.Where(puf => puf.ProjectId == projectId).ToList();

            if (projectUpdateDetails.Count > 0)
            {
                // send email of project details modification to current owners and collaborators
                _emailService.SaveProjectUpdatedOwnerToEmailQueue(currentProjectOwners.Select(x => x.UserId).ToList(), projectUpdateDetails, projectId, result.Entity.ProjectName, request.UserId);
                _emailService.SaveProjectUpdatedCollaboratorToEmailQueue(currentProjectUsers.Select(x => x.UserId).ToList(), projectUpdateDetails, projectId, result.Entity.ProjectName, request.UserId);
            }

            // Project Owners

            var currentProjectOwnerIds = currentProjectOwners.Select(po => po.UserId);
            var projectOwnersToAdd = new List<ProjectOwner>();

            foreach (var user in request.Project.ProjectOwners)
            {
                if (!currentProjectOwnerIds.Contains(user.UserId))
                {
                    var newProjectOwner = new ProjectOwner()
                    {
                        ProjectId = projectId,
                        UserId = user.UserId,
                        CreatedById = request.UserId
                    };

                    projectOwnersToAdd.Add(newProjectOwner);
                }
            }
            _dbcontext.ProjectOwners.AddRange(projectOwnersToAdd);
            _dbcontext.SaveChanges();

            // Send email to new project owners
            _emailService.SaveAddedOwnersToEmailQueue(projectOwnersToAdd.Select(x => x.UserId).ToList(), projectId, result.Entity.ProjectName, request.UserId);

            var updateProjectOwnerIds = request.Project.ProjectOwners.Select(u => u.UserId);
            var projectOwnersToRemove = new List<ProjectOwner>();

            foreach (var user in currentProjectOwners)
            {
                if (!updateProjectOwnerIds.Contains(user.UserId))
                {
                    projectOwnersToRemove.Add(user);
                }
            }

            _dbcontext.ProjectOwners.RemoveRange(projectOwnersToRemove);
            _dbcontext.SaveChanges();

            // Project Followers

            var currentProjectUserIds = currentProjectUsers.Select(puf => puf.UserId);
            var projectUsersToAdd = new List<ProjectUserFollower>();

            foreach (var user in request.Project.ProjectUserFollowers)
            {
                if (!currentProjectUserIds.Contains(user.UserId))
                {
                    var newProjectUser = new ProjectUserFollower()
                    {
                        ProjectId = projectId,
                        UserId = user.UserId,
                        CreatedById = request.UserId
                    };

                    projectUsersToAdd.Add(newProjectUser);
                }
            }
            _dbcontext.ProjectUserFollowers.AddRange(projectUsersToAdd);
            _dbcontext.SaveChanges();

            // Send email to new project coilaborators
            _emailService.SaveAddedCollaboratorsToEmailQueue(projectUsersToAdd.Select(x => x.UserId).ToList(), projectId, result.Entity.ProjectName, request.UserId);

            var updateProjectUserIds = request.Project.ProjectUserFollowers.Select(u => u.UserId);
            var projectUsersToRemove = new List<ProjectUserFollower>();

            foreach (var user in currentProjectUsers)
            {
                if (!updateProjectUserIds.Contains(user.UserId))
                {
                    projectUsersToRemove.Add(user);
                }
            }

            _dbcontext.ProjectUserFollowers.RemoveRange(projectUsersToRemove);
            _dbcontext.SaveChanges();

            // Project Teams

            var currentProjectTeams = _dbcontext.ProjectTeamFollowers.Where(ptf => ptf.ProjectId == projectId).ToList();
            var currentProjectTeamIds = currentProjectTeams.Select(ptf => ptf.TeamId);
            var projectTeamsToAdd = new List<ProjectTeamFollower>();

            foreach (var team in request.Project.ProjectTeamFollowers)
            {
                if (!currentProjectTeamIds.Contains(team.TeamId))
                {
                    var newProjectTeam = new ProjectTeamFollower()
                    {
                        ProjectId = projectId,
                        TeamId = team.TeamId,
                        CreatedById = request.UserId
                    };

                    projectTeamsToAdd.Add(newProjectTeam);
                }
            }
            _dbcontext.ProjectTeamFollowers.AddRange(projectTeamsToAdd);
            _dbcontext.SaveChanges();

            var updateProjectTeamIds = request.Project.ProjectTeamFollowers.Select(u => u.TeamId);
            var projectTeamsToRemove = new List<ProjectTeamFollower>();

            foreach (var team in currentProjectTeams)
            {
                if (!updateProjectTeamIds.Contains(team.TeamId))
                {
                    projectTeamsToRemove.Add(team);
                }
            }

            _dbcontext.ProjectTeamFollowers.RemoveRange(projectTeamsToRemove);
            _dbcontext.SaveChanges();

            result.ResultType = ResultType.Success;
            return result;
        }
    }
}
