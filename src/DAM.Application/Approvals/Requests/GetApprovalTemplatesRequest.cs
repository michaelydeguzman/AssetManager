using DAM.Application.Approval.Dtos;
using DAM.Application.Assets.Dtos;
using DAM.Domain.Entities;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace DAM.Application.Approvals.Requests
{
    public class GetApprovalTemplatesRequest : IRequest<HandlerResult<IEnumerable<ApprovalTemplateDto>>>
    {
        public int CompanyId { get; private set; }
        public GetApprovalTemplatesRequest(int companyId = 0) 
        {
            CompanyId = companyId;
        }
    }
}
