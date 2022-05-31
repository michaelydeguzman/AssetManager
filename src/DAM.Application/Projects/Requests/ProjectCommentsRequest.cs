using DAM.Application.Projects.Dtos;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace DAM.Application.Projects.Requests
{
    public class ProjectCommentsRequest : IRequest<HandlerResult<IEnumerable<ProjectCommentDto>>>
    {
        public int ProjectId { get; set; }
        public ProjectCommentsRequest(int projectId)
        {
            ProjectId = projectId;
        }
    }
}
