using AutoMapper;
using DAM.Application.Approvals.Dtos;
using DAM.Application.Cache;
using DAM.Persistence;
using DAM.Domain.Entities;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Linq;
using DAM.Application.Approvals.Requests;
using DAM.Application.Approval.Dtos;
using DAM.Application.Assets.Enums;
using Microsoft.EntityFrameworkCore;

namespace DAM.Application.Approvals.Handlers
{
    public class GetAssetApprovalsRequestHandler : HandlerBase<GetAssetApprovalsRequest, HandlerResult<IEnumerable<ApprovalLevelDto>>>
    {
        private readonly ICacheProvider _cacheProvider;
        private readonly IDbContext _dbcontext;
        private readonly IMapper _mapper;
        private readonly IConfiguration _configuration;

        public GetAssetApprovalsRequestHandler(ICacheProvider cacheProvider, IDbContext dbcontext, IMapper mapper, IConfiguration configuration)
        {
            _cacheProvider = cacheProvider ?? throw new ArgumentNullException(nameof(cacheProvider));
            _dbcontext = dbcontext ?? throw new ArgumentNullException(nameof(dbcontext));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
        }

        public override HandlerResult<IEnumerable<ApprovalLevelDto>> HandleRequest(GetAssetApprovalsRequest request, CancellationToken cancellationToken, HandlerResult<IEnumerable<ApprovalLevelDto>> result)
        {
            List<ApprovalLevel> approvalLevels = new List<ApprovalLevel>();
            var allUsers = _dbcontext.AppUsers.ToList();
            var asset = _dbcontext.Assets.FirstOrDefault(a => a.Id == request.AssetId && a.Status != (int)AssetStatus.Archived && a.Status != (int)AssetStatus.Deleted);
     
            if (asset != null && request.AssetVersionId > 0)
            {
                approvalLevels = _dbcontext.ApprovalLevels.Where(x => x.AssetId == asset.Id && x.AssetVersionId == request.AssetVersionId && x.IsActiveLevel != null).Include(x => x.Approvers).OrderBy(x => x.Id).ToList();
            } 
            else
            {
                approvalLevels = _dbcontext.ApprovalLevels.Where(x => x.AssetId == asset.Id && x.IsActiveLevel != null).Include(x => x.Approvers).OrderBy(x => x.Id).ToList();
            }

            result.Entity = _mapper.Map<IEnumerable<ApprovalLevelDto>>(approvalLevels);
            result.ResultType = ResultType.Success;

            foreach (var approvalLevel in result.Entity)
            {
                approvalLevel.CreatedByName = allUsers.First(u => u.Id == approvalLevel.CreatedById).UserName;
                if(!String.IsNullOrEmpty(approvalLevel.ModifiedById))
                {
                    approvalLevel.ModifiedById = allUsers.First(u => u.Id == approvalLevel.ModifiedById).UserName;
                }
                foreach (var approver in approvalLevel.Approvers)
                {
                    approver.ApproverName = allUsers.FirstOrDefault(x => x.Id == approver.ApproverId).UserName;
                };
            };
            return result;
        }

        public override Exception OnException(Exception ex)
        {
            return new Exception(ex.Message, ex);
        }
    }
}
