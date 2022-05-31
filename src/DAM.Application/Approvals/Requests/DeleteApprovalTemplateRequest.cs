using DAM.Application.Approvals.Dtos;
using DAM.Domain.Entities;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace DAM.Application.Approvals.Requests
{
    public class DeleteApprovalTemplateRequest : IRequest<HandlerResult<IEnumerable<ApprovalTemplateDto>>>
    {
        public ApprovalTemplateDto ApprovalTemplateDelete { get; private set; }
        public string UserId { get; set; }

        public DeleteApprovalTemplateRequest(ApprovalTemplateDto templateDto, string userId)
        {
            ApprovalTemplateDelete = templateDto;
            UserId = userId;
        }
    }
}
