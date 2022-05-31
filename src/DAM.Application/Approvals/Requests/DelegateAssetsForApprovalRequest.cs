using DAM.Application.Approvals.Dtos;
using DAM.Application.Assets.Dtos;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace DAM.Application.Approvals.Requests
{
    public class DelegateAssetsForApprovalRequest : IRequest<HandlerResult<DelegateAssetsForApprovalDto>>
    {
        public DelegateAssetsForApprovalDto DelegateAssetApprovals { get; private set; }
        
        public DelegateAssetsForApprovalRequest(DelegateAssetsForApprovalDto delegateAssetAppproval) 
        {
            DelegateAssetApprovals = delegateAssetAppproval;
        }
    }
}
