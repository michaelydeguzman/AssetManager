using DAM.Application.Projects.Dtos;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace DAM.Application.Projects.Requests
{
    public class GetProjectUploadsRequest : IRequest<HandlerResult<IEnumerable<int>>>
    {
        public int ProjectId { get; set; }
        public GetProjectUploadsRequest(int projectId)
        {
            ProjectId = projectId;
        }
    }
}
