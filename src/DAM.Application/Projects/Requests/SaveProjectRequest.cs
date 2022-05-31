using DAM.Application.Projects.Dtos;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace DAM.Application.Projects.Requests
{
    public class SaveProjectRequest : IRequest<HandlerResult<ProjectDto>>
    {
        public string UserId { get; set; }
        public ProjectDto Project { get; set; }
        public SaveProjectRequest(ProjectDto project, string userId)
        {
            Project = project;
            UserId = userId;
        }
    }
}
