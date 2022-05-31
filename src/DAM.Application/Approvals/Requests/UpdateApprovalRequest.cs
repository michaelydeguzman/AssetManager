using DAM.Application.Approvals.Dtos;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace DAM.Application.Approvals.Requests
{
    public class UpdateApprovalRequest : IRequest<HandlerResult<UpdateApprovalDto>>
    {
        public string UserId { get; private set; }

        public List<UpdateApprovalDto> ApprovalUpdate { get; private set; }

        public UpdateApprovalRequest(List<UpdateApprovalDto> approvalUpdate, string userId)
        {
            ApprovalUpdate = approvalUpdate;
            UserId = userId;
        }
    }
}
