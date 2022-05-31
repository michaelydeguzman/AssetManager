using DAM.Application.Projects.Dtos;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace DAM.Application.Projects.Requests
{
    public class UnarchiveProjectRequest : IRequest<HandlerResult<ProjectDto>>
    {
        public string UserId { get; set; }
        public int ProjectId { get; set; }
        public UnarchiveProjectRequest(int projectId, string userId)
        {
            ProjectId = projectId;
            UserId = userId;
        }
    }
}
