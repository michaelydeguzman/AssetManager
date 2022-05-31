using DAM.Application.Approval.Dtos;
using DAM.Application.Approvals.Dtos;
using DAM.Application.Assets.Dtos;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace DAM.Application.Approvals.Requests
{
    public class GetApprovalsOnOOORequest : IRequest<HandlerResult<IEnumerable<ApprovalsOnOOODto>>>
    {
        public GetApprovalsOnOOODto GetApprovalsOnOOO { get; set; }
        public GetApprovalsOnOOORequest(GetApprovalsOnOOODto getApprovalsOnOOO)
        {
            GetApprovalsOnOOO = getApprovalsOnOOO;
        }
    }
}
