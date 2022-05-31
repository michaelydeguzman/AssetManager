using AutoMapper;
using DAM.Application.Approvals.Dtos;
using DAM.Application.Approvals.Enums;
using DAM.Application.Approvals.Requests;
using DAM.Application.Assets.Enums;
using DAM.Application.AuditTrail.Dtos;
using DAM.Application.AuditTrail.Enums;
using DAM.Application.Cache;
using DAM.Application.Extensions;
using DAM.Application.Services.Interfaces;
using DAM.Domain.Entities;
using DAM.Persistence;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace DAM.Application.Approvals.Handlers
{
    public class UpdateApprovalRequestHandler : HandlerBase<UpdateApprovalRequest, HandlerResult<UpdateApprovalDto>>
    {
        private readonly ICacheProvider _cacheProvider;
        private readonly IDbContext _dbcontext;
        private readonly IMapper _mapper;
        private readonly IConfiguration _configuration;
        private readonly IHelperService _helperService;
        private readonly IEmailService _emailService;

        public UpdateApprovalRequestHandler(ICacheProvider cacheProvider, IDbContext dbcontext, IMapper mapper, IConfiguration configuration, IHelperService helperService, IEmailService emailService)
        {
            _cacheProvider = cacheProvider ?? throw new ArgumentNullException(nameof(cacheProvider));
            _dbcontext = dbcontext ?? throw new ArgumentNullException(nameof(dbcontext));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
            _helperService = helperService ?? throw new ArgumentNullException(nameof(helperService));
            _emailService = emailService ?? throw new ArgumentNullException(nameof(emailService));
        }
        public override HandlerResult<UpdateApprovalDto> HandleRequest(UpdateApprovalRequest request, CancellationToken cancellationToken, HandlerResult<UpdateApprovalDto> result)
        {
            foreach (var approvalRequest in request.ApprovalUpdate)
            {
                var assetId = approvalRequest.AssetId;
                var assetVersionId = approvalRequest.AssetVersionId;
                var approver = _dbcontext.AppUsers.FirstOrDefault(u => u.Id == request.UserId);
                var reviewDate = DateTimeOffset.UtcNow;

                // update approval level approver status
                var approvalLevelApprover = (from userApprovals in _dbcontext.ApprovalLevelApprovers
                                             join assetApprovalLevels in _dbcontext.ApprovalLevels on userApprovals.ApprovalLevelId equals assetApprovalLevels.Id
                                             join assetVersionForApproval in _dbcontext.AssetVersions on assetApprovalLevels.AssetVersionId equals assetVersionForApproval.Id
                                             where assetApprovalLevels.AssetId == assetId
                                                && assetApprovalLevels.AssetVersionId == assetVersionId
                                                && userApprovals.ApproverId == approver.Id
                                                && userApprovals.ApprovalStatus == (int)ApprovalStatus.Pending
                                                && userApprovals.ReviewDate == null
                                                && assetApprovalLevels.IsActiveLevel == true && assetApprovalLevels.CompletedDate == null
                                                && assetVersionForApproval.Id == assetVersionId
                                                && assetVersionForApproval.Status == (int)AssetStatus.Submitted
                                             select userApprovals).FirstOrDefault();
                approvalLevelApprover.ApprovalStatus = (int)(approvalRequest.IsApproved ? ApprovalStatus.Approved : ApprovalStatus.Rejected);
                approvalLevelApprover.Comments = approvalRequest.Comment;
                approvalLevelApprover.ReviewDate = reviewDate;
                _dbcontext.ApprovalLevelApprovers.Update(approvalLevelApprover);
                _dbcontext.SaveChanges();

                var asset = _dbcontext.Assets.FirstOrDefault(x => x.Id == approvalRequest.AssetId);
                var version = _dbcontext.AssetVersions.FirstOrDefault(a => a.Id == approvalRequest.AssetVersionId);
                var folder = _dbcontext.Folders.FirstOrDefault(x => x.Id == asset.FolderId);

                var approversToEmail = new List<ApprovalLevelApprover>();
                ApprovalLevel nextApprovalLevelToEmail = null;

                if (!approvalRequest.IsApproved)
                {
                    // Auto-Reject whole asset
                    asset.Status = (int)AssetStatus.Rejected;
                    asset.StatusUpdatedDate = reviewDate;
                    version.Status = (int)AssetStatus.Rejected;
                    version.StatusUpdatedDate = reviewDate;
                    _dbcontext.Assets.Update(asset);
                    _dbcontext.AssetVersions.Update(version);
                    _dbcontext.SaveChanges();
                }
                else
                {
                    // get current approval level
                    var currentApprovalLevel = _dbcontext.ApprovalLevels.FirstOrDefault(x => x.AssetId == assetId && x.AssetVersionId == assetVersionId && x.IsActiveLevel == true);

                    // check if current approval has no more unactioned appprovers
                    var hasPendingApprovers = _dbcontext.ApprovalLevelApprovers.Any(x => x.ApprovalLevelId == currentApprovalLevel.Id && x.ApprovalStatus == (int)ApprovalStatus.Pending);

                    if (!hasPendingApprovers)
                    {
                        // set active level to false
                        currentApprovalLevel.IsActiveLevel = false;
                        currentApprovalLevel.CompletedDate = reviewDate;
                        _dbcontext.ApprovalLevels.Update(currentApprovalLevel);
                        _dbcontext.SaveChanges();

                        // move to next level if exist, else update asset status
                        var allApprovalLevels = _dbcontext.ApprovalLevels.ToList();

                        var isMaxLevel = allApprovalLevels.Max(x => x.Id) == currentApprovalLevel.Id;

                        var nextApprovalLevels = new List<ApprovalLevel>();

                        if (!isMaxLevel)
                        {
                            nextApprovalLevels = allApprovalLevels.Where(x => x.AssetId == assetId && x.AssetVersionId == assetVersionId && x.Id > currentApprovalLevel.Id && x.CompletedDate == null).ToList();
                        }

                        if (nextApprovalLevels.Count > 0)
                        {
                            var nextApprovalLevel = nextApprovalLevels.Min();
                            nextApprovalLevel.IsActiveLevel = true;
                            if (nextApprovalLevel.Duration.HasValue)
                            {
                                nextApprovalLevel.DueDate = _helperService.GetDateWithBusinessDays(currentApprovalLevel.CompletedDate.Value, nextApprovalLevel.Duration.Value);
                            }

                            _dbcontext.ApprovalLevels.Update(nextApprovalLevel);
                            _dbcontext.SaveChanges();

                            nextApprovalLevelToEmail = nextApprovalLevel;

                            // OOO Delegation
                            var nextApprovalLevelApprovers = _dbcontext.ApprovalLevelApprovers.Where(a => a.ApprovalLevelId == nextApprovalLevel.Id).ToList();
                            var nextApprovalLevelApproverIds = nextApprovalLevelApprovers.Select(x => x.ApproverId).ToList();
                            var approversToRemove = new List<ApprovalLevelApprover>();

                            foreach (var nxtlvlApprover in nextApprovalLevelApprovers)
                            {
                                var approverOOO = _dbcontext.UserOOO.FirstOrDefault(o => !o.Deleted && o.UserId == nxtlvlApprover.ApproverId
                              && nextApprovalLevel.DueDate >= o.StartDate && nextApprovalLevel.DueDate <= o.EndDate);

                                if (approverOOO != null && !string.IsNullOrEmpty(approverOOO.DefaultDelegateUser))
                                {
                                    if (!nextApprovalLevelApproverIds.Contains(approverOOO.DefaultDelegateUser))
                                    {
                                        nxtlvlApprover.ApproverId = approverOOO.DefaultDelegateUser;
                                        _dbcontext.ApprovalLevelApprovers.Update(nxtlvlApprover);
                                        _dbcontext.SaveChanges();
                                    }
                                    else
                                    {
                                        // remove approver since delegate is already part of next level approvers
                                        approversToRemove.Add(nxtlvlApprover);
                                    }

                                    // insert into delegation table?
                                }
                            }

                            if (approversToRemove.Count > 0)
                            {
                                _dbcontext.ApprovalLevelApprovers.RemoveRange(approversToRemove);
                                _dbcontext.SaveChanges();
                            }

                            approversToEmail = _dbcontext.ApprovalLevelApprovers.Where(a => a.ApprovalLevelId == nextApprovalLevel.Id && a.ReviewDate == null).ToList();

                        }
                        else
                        {
                            asset.Status = (int)AssetStatus.Approved;
                            asset.StatusUpdatedDate = reviewDate;
                            version.Status = (int)AssetStatus.Approved;
                            version.StatusUpdatedDate = reviewDate;
                            _dbcontext.Assets.Update(asset);
                            _dbcontext.AssetVersions.Update(version);
                            _dbcontext.SaveChanges();
                        }
                    }
                }


                if (asset.Status == (int)AssetStatus.Approved)
                {
                    // Send approved email
                    _emailService.SaveApprovedEmailToEmailQueue(asset, version, request.UserId, approvalLevelApprover.CreatedById, approvalRequest.Comment, false, folder.Id, folder.FolderName);
                }
                else
                {
                    //// Send rejected email
                    _emailService.SaveRejectedEmailToEmailQueue(asset, version, request.UserId, approvalLevelApprover.CreatedById, approvalRequest.Comment, false, folder.Id, folder.FolderName);
                }


                // Send approvers email
                if (nextApprovalLevelToEmail != null)
                {
                    _emailService.SaveApprovalEmailToEmailQueue(nextApprovalLevelToEmail, approversToEmail, request.UserId, false, folder.Id, folder.FolderName);
                }

                // Insert Audit Trail
                var auditDetail = new ApproverActionAuditDto()
                {
                    ApproverName = approver.UserName,
                    Comments = approvalRequest.Comment
                };
                var auditTrailEntry = new AssetAudit()
                {
                    AssetId = Convert.ToInt32(asset.Id),
                    FolderId = asset.FolderId,
                    AssetFileName = version.FileName,
                    AuditType = Convert.ToInt32(approvalRequest.IsApproved ? AssetAuditType.AssetApproved : AssetAuditType.AssetRejected),
                    AuditTypeText = (approvalRequest.IsApproved ? AssetAuditType.AssetApproved : AssetAuditType.AssetRejected).GetDescription(),
                    AuditCreatedByUserId = approver.Id,
                    AuditCreatedDate = reviewDate,
                    AuditCreatedByName = approver != null ? approver.UserName : "",
                    NewParameters = _helperService.GetJsonString(auditDetail)
                };

                _dbcontext.AssetAudit.Add(auditTrailEntry);
                _dbcontext.SaveChanges();
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
