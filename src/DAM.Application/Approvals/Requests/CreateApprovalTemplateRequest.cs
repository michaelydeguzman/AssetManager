using DAM.Application.Approvals.Dtos;
using DAM.Domain.Entities;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace DAM.Application.Approvals.Requests
{
    public class CreateApprovalTemplateRequest : IRequest<HandlerResult<IEnumerable<ApprovalTemplateDto>>>
    {
        public ApprovalTemplateDto ApprovalTemplateCreate{ get; private set; }
        public string UserId { get; set; }

        public CreateApprovalTemplateRequest(ApprovalTemplateDto approvalTemplateCreate, string userId)
        {
            ApprovalTemplateCreate = approvalTemplateCreate;
            UserId = userId;
        }
    }
}
