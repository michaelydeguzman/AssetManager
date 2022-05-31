using DAM.Application.Projects.Dtos;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace DAM.Application.Projects.Requests
{
    public class ProjectOwnersRequest : IRequest<HandlerResult<IEnumerable<ProjectOwnerDto>>>
    {
        public int ProjectId { get; set; }
        public ProjectOwnersRequest(int projectId)
        {
            ProjectId = projectId;
        }
    }
}
