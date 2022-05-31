using DAM.Application.Projects.Dtos;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace DAM.Application.Projects.Requests
{
    public class GetImportedProjectAssetsRequest : IRequest<HandlerResult<IEnumerable<ProjectItemDto>>>
    {
        public int ProjectId { get; set; }
        public GetImportedProjectAssetsRequest(int projectId)
        {
            ProjectId = projectId;
        }
    }
}
