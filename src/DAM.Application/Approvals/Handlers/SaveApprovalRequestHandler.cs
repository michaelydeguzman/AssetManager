using AutoMapper;
using DAM.Application.Approval.Dtos;
using DAM.Application.Approvals.Dtos;
using DAM.Application.Approvals.Requests;
using DAM.Application.Assets.Dtos;
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
    public class SaveApprovalRequestHandler : HandlerBase<SaveApprovalRequest, HandlerResult<AssetApprovalDto>>
    {
        private readonly ICacheProvider _cacheProvider;
        private readonly IDbContext _dbcontext;
        private readonly IMapper _mapper;
        private readonly IConfiguration _configuration;
        private readonly IHelperService _helperService;
        private readonly IEmailService _emailService;

        public SaveApprovalRequestHandler(ICacheProvider cacheProvider, IDbContext dbcontext, IMapper mapper, IConfiguration configuration, IHelperService helperService, IEmailService emailService)
        {
            _cacheProvider = cacheProvider ?? throw new ArgumentNullException(nameof(cacheProvider));
            _dbcontext = dbcontext ?? throw new ArgumentNullException(nameof(dbcontext));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
            _helperService = helperService ?? throw new ArgumentNullException(nameof(helperService));
            _emailService = emailService ?? throw new ArgumentNullException(nameof(emailService));
        }
        public override HandlerResult<AssetApprovalDto> HandleRequest(SaveApprovalRequest request, CancellationToken cancellationToken, HandlerResult<AssetApprovalDto> result)
        {
            var createdBy = request.UserId;
            var applyOOODelegation = request.Approvals.DelegateIfOOO;

            var asset = _dbcontext.Assets.FirstOrDefault(x => x.Id == request.Approvals.AssetId);
            var folder = _dbcontext.Folders.FirstOrDefault(x => x.Id == asset.FolderId);

            foreach (var approvalLevel in request.Approvals.ApprovalLevels)
            {
                if (approvalLevel.Id == null) // new approval level
                {
                    // insert new approval level & approver level approvers
                    var newApprovalLevel = _mapper.Map<ApprovalLevel>(approvalLevel);
                    newApprovalLevel.CreatedById = createdBy;

                    if (newApprovalLevel.Duration.HasValue && newApprovalLevel.IsActiveLevel.Value == true)
                    {
                        newApprovalLevel.DueDate = _helperService.GetDateWithBusinessDays(newApprovalLevel.CreatedDate, newApprovalLevel.Duration.Value);
                    }

                    _dbcontext.ApprovalLevels.Add(newApprovalLevel);

                    _dbcontext.SaveChanges();

                    var newApproverIds = approvalLevel.Approvers.Select(a => a.ApproverId).ToList();
                    var newApprovers = new List<ApprovalLevelApprover>();
                    var emailList = new List<ApprovalLevelApprover>();

                    foreach (var approver in approvalLevel.Approvers)
                    {
                        var newApprover = _mapper.Map<ApprovalLevelApprover>(approver);
                        newApprover.ApprovalLevelId = newApprovalLevel.Id;
                        newApprover.CreatedById = createdBy;

                        if (applyOOODelegation && newApprovalLevel.IsActiveLevel.Value)
                        {
                            // check if user has ooo hit and reassign approval to delegate user if any
                            var approverOOO = _dbcontext.UserOOO.FirstOrDefault(o => !o.Deleted && o.UserId == newApprover.ApproverId
                            && newApprovalLevel.DueDate >= o.StartDate && newApprovalLevel.DueDate <= o.EndDate);

                            if (approverOOO != null && !string.IsNullOrEmpty(approverOOO.DefaultDelegateUser))
                            {
                                if (!newApproverIds.Contains(approverOOO.DefaultDelegateUser))
                                {
                                    newApprover.ApproverId = approverOOO.DefaultDelegateUser;
                                    newApprovers.Add(newApprover);

                                }
                                // insert into delegation table?
                            }
                            else
                            {
                                newApprovers.Add(newApprover);
                            }
                        }
                        else
                        {
                            newApprovers.Add(newApprover);
                        }
                    }

                    _dbcontext.ApprovalLevelApprovers.AddRange(newApprovers);
                    _dbcontext.SaveChanges();

                    // Send out emails to approvers
                    if (newApprovalLevel.IsActiveLevel.Value)
                    {
                        _emailService.SaveApprovalEmailToEmailQueue(newApprovalLevel, newApprovers, request.UserId, false, folder.Id, folder.FolderName);
                    }
                }
                else // existing approval level
                {
                    var existingApprovers = _dbcontext.ApprovalLevelApprovers.Where(x => x.ApprovalLevelId == approvalLevel.Id).ToList();

                    // add new approvers
                    var existingApproverIds = existingApprovers.Select(x => x.ApproverId);
                    var approversToAdd = approvalLevel.Approvers.Where(x => x.Id == null && !existingApproverIds.Contains(x.ApproverId)).ToList();
                    _dbcontext.ApprovalLevelApprovers.AddRange(_mapper.Map<List<ApprovalLevelApprover>>(approversToAdd));

                    // check if approvers are deleted
                    var approverIdsFromClient = approvalLevel.Approvers.Select(x => x.ApproverId).ToList();
                    var approversToDelete = existingApprovers.Where(x => !approverIdsFromClient.Contains(x.ApproverId)).ToList();
                    _dbcontext.ApprovalLevelApprovers.RemoveRange(approversToDelete);

                    _dbcontext.SaveChanges();
                }

                // Check if submitted for approval and change Asset Status to Submitted if true
                if (request.Approvals.isSubmitted == true)
                {
                    var version = _dbcontext.AssetVersions.FirstOrDefault(a => a.Id == approvalLevel.AssetVersionId && a.AssetId == approvalLevel.AssetId);
                    version.Status = (int)AssetStatus.Submitted;
                    version.StatusUpdatedDate = DateTimeOffset.UtcNow;
                    _dbcontext.AssetVersions.Update(version);
                    _dbcontext.SaveChanges();

                    var assetItem = _dbcontext.Assets.First(x => x.Id == approvalLevel.AssetId);
                    assetItem.Status = (int)AssetStatus.Submitted;
                    assetItem.StatusUpdatedDate = DateTimeOffset.UtcNow;

                    _dbcontext.Assets.Update(assetItem);
                    _dbcontext.SaveChanges();

                    var auditDisplayUser = _dbcontext.AppUsers.First(u => u.Id == createdBy);
                    var auditTrailEntry = new AssetAudit()
                    {
                        AssetId = Convert.ToInt32(assetItem.Id),
                        FolderId = assetItem.FolderId,
                        AssetFileName = version.FileName,
                        AuditType = Convert.ToInt32(AssetAuditType.AssetSubmitted),
                        AuditTypeText = AssetAuditType.AssetSubmitted.GetDescription(),
                        AuditCreatedByUserId = assetItem.CreatedById,
                        AuditCreatedDate = DateTimeOffset.UtcNow,
                        AuditCreatedByName = auditDisplayUser != null ? auditDisplayUser.UserName : "",
                        NewParameters = "send the asset to level " + approvalLevel.LevelNumber.ToString() + " approval."
                    };
                    _dbcontext.AssetAudit.Add(auditTrailEntry);
                    _dbcontext.SaveChanges();
                }
            }

            // Check if submitted for approval and change Asset Status to Submitted if true
            //if (request.Approvals.isSubmitted == true)
            //{
            //var version = _dbcontext.AssetVersions.FirstOrDefault(a => a.Id == request.Approvals.AssetVersionId && a.AssetId == request.Approvals.AssetId);
            //version.Status = (int)AssetStatus.Submitted;
            //version.StatusUpdatedDate = DateTimeOffset.UtcNow;

            //_dbcontext.AssetVersions.Update(version);
            //_dbcontext.SaveChanges();

            //var asset = _dbcontext.Assets.First(x => x.Id == request.Approvals.AssetId);
            //asset.Status = (int)AssetStatus.Submitted;
            //asset.StatusUpdatedDate = DateTimeOffset.UtcNow;

            //_dbcontext.Assets.Update(asset);
            //_dbcontext.SaveChanges();

            //var auditDisplayUser = _dbcontext.AppUsers.First(u => u.Id == createdBy);

            //var auditTrailEntry = new AssetAudit()
            //{
            //    AssetId = Convert.ToInt32(asset.Id),
            //    FolderId = asset.FolderId,
            //    AssetFileName = version.FileName,
            //    AuditType = Convert.ToInt32(AssetAuditType.AssetSubmitted),
            //    AuditTypeText = AssetAuditType.AssetSubmitted.GetDescription(),
            //    AuditCreatedByUserId = asset.CreatedById,
            //    AuditCreatedDate = DateTimeOffset.UtcNow,
            //    AuditCreatedByName = auditDisplayUser != null ? auditDisplayUser.UserName : "",
            //};

            //_dbcontext.AssetAudit.Add(auditTrailEntry);
            //_dbcontext.SaveChanges();
            //}
            result.ResultType = ResultType.Success;
            return result;
        }
        public override Exception OnException(Exception ex)
        {
            return new Exception(ex.Message, ex);
        }



    }
}
