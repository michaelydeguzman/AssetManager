using DAM.Application.Approvals.Dtos;
using DAM.Application.Assets.Dtos;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace DAM.Application.Approvals.Requests
{
    public class SaveApprovalRequest : IRequest<HandlerResult<AssetApprovalDto>>
    {
        public string UserId { get; private set; }
        public AssetApprovalDto Approvals { get; private set; }
        public SaveApprovalRequest(AssetApprovalDto approvals, string userId) 
        {
            Approvals = approvals;
            UserId = userId;
        }
    }
}
