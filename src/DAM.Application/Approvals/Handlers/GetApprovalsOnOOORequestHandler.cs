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
    public class GetApprovalsOnOOORequestHandler : HandlerBase<GetApprovalsOnOOORequest, HandlerResult<IEnumerable<ApprovalsOnOOODto>>>
    {
        private readonly ICacheProvider _cacheProvider;
        private readonly IDbContext _dbcontext;
        private readonly IMapper _mapper;
        private readonly IConfiguration _configuration;
        private readonly IHelperService _helperService;

        public GetApprovalsOnOOORequestHandler(ICacheProvider cacheProvider, IDbContext dbcontext, IMapper mapper, IConfiguration configuration, IHelperService helperService)
        {
            _cacheProvider = cacheProvider ?? throw new ArgumentNullException(nameof(cacheProvider));
            _dbcontext = dbcontext ?? throw new ArgumentNullException(nameof(dbcontext));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
            _helperService = helperService ?? throw new ArgumentNullException(nameof(helperService));

        }
        public override HandlerResult<IEnumerable<ApprovalsOnOOODto>> HandleRequest(GetApprovalsOnOOORequest request, CancellationToken cancellationToken, HandlerResult<IEnumerable<ApprovalsOnOOODto>> result)
        {
            result.Entity = new List<ApprovalsOnOOODto>();

            var approverId = request.GetApprovalsOnOOO.ApproverId;
            var startDate = request.GetApprovalsOnOOO.StartDate;
            var endDate = request.GetApprovalsOnOOO.EndDate;
            var allUserPendingApprovals = _dbcontext.ApprovalLevelApprovers.Where(a => a.ApproverId == approverId && a.ReviewDate == null).ToList();
            var allUserPendingApprovalsIds = allUserPendingApprovals.Select(x=>x.ApprovalLevelId).ToList();
            var allUserActiveApprovalLevels = _dbcontext.ApprovalLevels.Where(a=>allUserPendingApprovalsIds.Contains(a.Id) && a.CompletedDate == null && a.IsActiveLevel.Value).ToList();
            var allAssetWithPendingApproval = _dbcontext.Assets.Where(a => a.Status == (int)AssetStatus.Submitted).ToList();
            var allAssetWithPendingApprovalIds = allAssetWithPendingApproval.Select(x => x.Id).ToList();
            var allActiveAssetVersions = _dbcontext.AssetVersions.Where(av => av.ActiveVersion == 1 && allAssetWithPendingApprovalIds.Contains(av.AssetId)).ToList();
            var allCompanies = _dbcontext.Companies.Where(c=>c.Status).ToList();

            var approvalLevelApprovers = (from ala in allUserPendingApprovals
                                          join al in allUserActiveApprovalLevels
                                          on ala.ApprovalLevelId equals al.Id
                                          join a in allAssetWithPendingApproval
                                          on al.AssetId equals a.Id
                                          join av in allActiveAssetVersions
                                          on al.AssetVersionId equals av.Id
                                          where al.DueDate.HasValue && (al.DueDate.Value >= startDate && al.DueDate <= endDate)
                                          && a.Id == av.AssetId
                                          select new ApprovalsOnOOODto
                                          {
                                              AssetId = al.AssetId,
                                              AssetVersionId = al.AssetVersionId,
                                              Title = a.Title,
                                              FileName = av.FileName,
                                              Version = av.Version,
                                              LevelNumber = al.LevelNumber,
                                              ApprovalLevelId = al.Id,
                                              ApprovalLevelApproverId = ala.Id,
                                              ApproverId = ala.ApproverId,
                                              DueDate = al.DueDate.Value,
                                              FolderId = a.FolderId
                                          }).ToList();
            
            foreach(var level in approvalLevelApprovers)
            {
                if (level.FolderId.HasValue)
                {
                    var company = allCompanies.FirstOrDefault(f => f.RootFolderId == level.FolderId.Value);
                    if (company != null) {
                        level.CompanyId = company.Id;
                    }
                }
            }

            result.Entity = approvalLevelApprovers;
            result.ResultType = ResultType.Success;
            return result;
        }
        public override Exception OnException(Exception ex)
        {
            return new Exception(ex.Message, ex);
        }
    }
}
