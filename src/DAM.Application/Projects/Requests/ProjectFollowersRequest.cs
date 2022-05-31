using DAM.Application.Projects.Dtos;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace DAM.Application.Projects.Requests
{
    public class ProjectFollowersRequest : IRequest<HandlerResult<ProjectFollowersDto>>
    {
        public int ProjectId { get; set; }
        public ProjectFollowersRequest(int projectId)
        {
            ProjectId = projectId;
        }
    }
}
