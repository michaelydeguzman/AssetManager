using DAM.Application.Projects.Dtos;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace DAM.Application.Projects.Requests
{
    public class ProjectsRequest : IRequest<HandlerResult<IEnumerable<ProjectDto>>>
    {
        public string UserId { get; set; }
        public int ProjectId { get; set; }
        public bool IsArchived { get; set; }
        public ProjectsRequest(int projectId, string userId, bool isArchived = false)
        {
            ProjectId = projectId;
            UserId = userId;
            IsArchived = isArchived;
        }
    }
}
