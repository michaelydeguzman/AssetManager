using AutoMapper;
using DAM.Application.Approval.Dtos;
using DAM.Application.Approvals.Dtos;
using DAM.Application.Approvals.Requests;
using DAM.Application.Assets.Enums;
using DAM.Application.AuditTrail.Dtos;
using DAM.Application.AuditTrail.Enums;
using DAM.Application.Cache;
using DAM.Application.Extensions;
using DAM.Application.Services.Interfaces;
using DAM.Domain.Entities;
using DAM.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace DAM.Application.Approvals.Handlers
{
    public class DelegateAssetsForApprovalRequestHandler : HandlerBase<DelegateAssetsForApprovalRequest, HandlerResult<DelegateAssetsForApprovalDto>>
    {
        private readonly ICacheProvider _cacheProvider;
        private readonly IDbContext _dbcontext;
        private readonly IMapper _mapper;
        private readonly IConfiguration _configuration;
        private readonly IHelperService _helperService;
        private readonly IEmailService _emailService;

        public DelegateAssetsForApprovalRequestHandler(ICacheProvider cacheProvider, IDbContext dbcontext, IMapper mapper, IConfiguration configuration,
            IHelperService helperService, IEmailService emailService)
        {
            _cacheProvider = cacheProvider ?? throw new ArgumentNullException(nameof(cacheProvider));
            _dbcontext = dbcontext ?? throw new ArgumentNullException(nameof(dbcontext));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
            _helperService = helperService ?? throw new ArgumentNullException(nameof(helperService));
            _emailService = emailService ?? throw new ArgumentNullException(nameof(emailService));
        }
        public override HandlerResult<DelegateAssetsForApprovalDto> HandleRequest(DelegateAssetsForApprovalRequest request, CancellationToken cancellationToken, 
            HandlerResult<DelegateAssetsForApprovalDto> result)
        {
            result.Entity = request.DelegateAssetApprovals;
            var allApprovalLevels = _dbcontext.ApprovalLevels.ToList();

            foreach(var delegation in request.DelegateAssetApprovals.DelegateApprovals)
            {
                var approvalLevelApprover = _dbcontext.ApprovalLevelApprovers.FirstOrDefault(x => x.Id == delegation.ApprovalLevelApproverId 
                && x.ApproverId == delegation.ApproverId && x.ApprovalLevelId == delegation.ApprovalLevelId && x.ReviewDate == null);

                if (approvalLevelApprover == null)
                {
                    result.ResultType = ResultType.NoData;
                    return result;
                }

                approvalLevelApprover.ApproverId = delegation.DelegateId;
                approvalLevelApprover.ModifiedById = delegation.ApproverId;
                approvalLevelApprover.ModifiedDate = DateTimeOffset.UtcNow;

                _dbcontext.ApprovalLevelApprovers.Update(approvalLevelApprover);
                _dbcontext.SaveChanges();

                // send delegation email
                var approvalLevel = allApprovalLevels.First(l => l.Id == approvalLevelApprover.ApprovalLevelId);
                _emailService.SaveDelegateApprovalEmailToEmailQueue(approvalLevel, delegation.DelegateId, delegation.ApproverId);
            }

            result.ResultType = ResultType.Success;
            return result;
        }
        public override Exception OnException(Exception ex)
        {
            return new Exception(ex.Message, ex);
        }
    }
}
