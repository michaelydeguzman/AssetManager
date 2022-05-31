using DAM.Application.Approvals.Dtos;
using DAM.Domain.Entities;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace DAM.Application.Approvals.Requests
{
    public class UpdateApprovalTemplateRequest : IRequest<HandlerResult<IEnumerable<ApprovalTemplateDto>>>
    {
        public ApprovalTemplateDto ApprovalTemplateUpdate { get; private set; }
        public string UserId { get; set; }

        public UpdateApprovalTemplateRequest(ApprovalTemplateDto approvalTemplateUpdate, string userId)
        {
            ApprovalTemplateUpdate = approvalTemplateUpdate;
            UserId = userId;
        }
    }
}
