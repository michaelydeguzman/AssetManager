using AutoMapper;
using DAM.Application.Approval.Dtos;
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
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace DAM.Application.Approvals.Handlers
{
    public class UpdateApprovalTemplateRequestHandler : HandlerBase<UpdateApprovalTemplateRequest, HandlerResult<IEnumerable<ApprovalTemplateDto>>>
    {
        private readonly ICacheProvider _cacheProvider;
        private readonly IDbContext _dbcontext;
        private readonly IMapper _mapper;
        private readonly IConfiguration _configuration;
        private readonly IHelperService _helperService;

        public UpdateApprovalTemplateRequestHandler(ICacheProvider cacheProvider, IDbContext dbcontext, IMapper mapper, IConfiguration configuration, IHelperService helperService)
        {
            _cacheProvider = cacheProvider ?? throw new ArgumentNullException(nameof(cacheProvider));
            _dbcontext = dbcontext ?? throw new ArgumentNullException(nameof(dbcontext));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
            _helperService = helperService ?? throw new ArgumentNullException(nameof(helperService));
        }
        public override HandlerResult<IEnumerable<ApprovalTemplateDto>> HandleRequest(UpdateApprovalTemplateRequest request, CancellationToken cancellationToken, HandlerResult<IEnumerable<ApprovalTemplateDto>> result)
        {
            result.Entity = new List<ApprovalTemplateDto>();
             
            var updateTemplate = request.ApprovalTemplateUpdate;

            var template = _dbcontext.ApprovalTemplates.FirstOrDefault(x => x.Id == updateTemplate.Id);

            if (template == null)
            {
                result.ResultType = ResultType.NoData;
                return result;
            }

            template.TemplateName = updateTemplate.TemplateName;
            template.ModifiedById = request.UserId;
            template.ModifiedDate = DateTimeOffset.UtcNow;

            _dbcontext.ApprovalTemplates.Update(template);
            _dbcontext.SaveChanges();

            // Delete approval levels
            var existingTemplateLevels = _dbcontext.ApprovalTemplateLevels.Where(x => x.ApprovalTemplateId == template.Id);
            var levelOrderNumbers = updateTemplate.ApprovalTemplateLevels.Select(x => x.LevelOrderNumber);

            var levelsToDelete = new List<ApprovalTemplateLevel>();

            foreach (var level in existingTemplateLevels)
            {
                if (!levelOrderNumbers.Contains(level.LevelOrderNumber))
                {
                    levelsToDelete.Add(level);
                }
            }
            _dbcontext.ApprovalTemplateLevels.RemoveRange(levelsToDelete);
            _dbcontext.SaveChanges();

            // Process approval levels
            foreach (var level in updateTemplate.ApprovalTemplateLevels)
            {
                var levelId = 0;
                if (level.Id.HasValue)
                {
                    levelId = level.Id.Value;
                }
                else
                {
                    var existingLevel = _dbcontext.ApprovalTemplateLevels.FirstOrDefault(x => x.LevelOrderNumber == level.LevelOrderNumber && x.ApprovalTemplateId == template.Id);

                    if (existingLevel == null)
                    {
                        // create new

                        var newLevel = new ApprovalTemplateLevel()
                        {
                            ApprovalTemplateId = template.Id,
                            LevelOrderNumber = level.LevelOrderNumber
                        };
                        _dbcontext.ApprovalTemplateLevels.Add(newLevel);
                        _dbcontext.SaveChanges();

                        levelId = newLevel.Id;
                    } 
                    else
                    {
                        // edit existing level
                        levelId = existingLevel.Id;
                    }
                }

                UpdateLevel(levelId, level, request.UserId);
            }

            result.Entity = _mapper.Map<IEnumerable<ApprovalTemplateDto>>(GetApprovalTemplates(request.ApprovalTemplateUpdate.CompanyId.Value));
            result.ResultType = ResultType.Success;
            return result;
        }
        public override Exception OnException(Exception ex)
        {
            return new Exception(ex.Message, ex);
        }

        public void UpdateLevel(int levelId, ApprovalTemplateLevelDto level, string userId)
        {
            var newApprovers = new List<ApprovalTemplateLevelApprover>();
            var deleteApprovers = new List<ApprovalTemplateLevelApprover>();

            var existingLevelApprovers = _dbcontext.ApprovalTemplateLevelApprovers.Where(x => x.ApprovalTemplateLevelId == levelId).Select(x => x.ApproverId);
            var newApproverIds = level.Approvers.Select(x => x.ApproverId);

            // Delete approvers
            foreach (var approverId in existingLevelApprovers)
            {
                if (!newApproverIds.Contains(approverId))
                {
                    var deleteApprover = _dbcontext.ApprovalTemplateLevelApprovers.FirstOrDefault(x => x.ApproverId == approverId && x.ApprovalTemplateLevelId == levelId);
                    deleteApprovers.Add(deleteApprover);
                }
            }

            // Add new approvers
            foreach (var approver in level.Approvers)
            {
                var approverId = approver.ApproverId;

                if (!existingLevelApprovers.Contains(approverId))
                {
                    var newApprover = new ApprovalTemplateLevelApprover()
                    {
                        ApproverId = approverId,
                        ApprovalTemplateLevelId = levelId,
                        CreatedById = userId,
                    };
                    newApprovers.Add(newApprover);
                }
            }

            _dbcontext.ApprovalTemplateLevelApprovers.RemoveRange(deleteApprovers);
            _dbcontext.ApprovalTemplateLevelApprovers.AddRange(newApprovers);
            _dbcontext.SaveChanges();
        }

        public IEnumerable<ApprovalTemplate> GetApprovalTemplates(int companyId)
        {
            return _dbcontext.ApprovalTemplates.Where(x => x.CompanyId == companyId && !x.isDeleted).Include(x => x.ApprovalTemplateLevels).ThenInclude(x => x.Approvers).ToList();
        }
    }
}
