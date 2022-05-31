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
    public class CreateApprovalTemplateRequestHandler : HandlerBase<CreateApprovalTemplateRequest, HandlerResult<IEnumerable<ApprovalTemplateDto>>>
    {
        private readonly ICacheProvider _cacheProvider;
        private readonly IDbContext _dbcontext;
        private readonly IMapper _mapper;
        private readonly IConfiguration _configuration;
        private readonly IHelperService _helperService;

        public CreateApprovalTemplateRequestHandler(ICacheProvider cacheProvider, IDbContext dbcontext, IMapper mapper, IConfiguration configuration, IHelperService helperService)
        {
            _cacheProvider = cacheProvider ?? throw new ArgumentNullException(nameof(cacheProvider));
            _dbcontext = dbcontext ?? throw new ArgumentNullException(nameof(dbcontext));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
            _helperService = helperService ?? throw new ArgumentNullException(nameof(helperService));
        }
        public override HandlerResult<IEnumerable<ApprovalTemplateDto>> HandleRequest(CreateApprovalTemplateRequest request, CancellationToken cancellationToken,
            HandlerResult<IEnumerable<ApprovalTemplateDto>> result)
        {
            result.Entity = new List<ApprovalTemplateDto>();

            var exists = _dbcontext.ApprovalTemplates.Any(x => x.CompanyId == request.ApprovalTemplateCreate.CompanyId && x.TemplateName == request.ApprovalTemplateCreate.TemplateName && !x.isDeleted);

            if (exists)
            {
                result.ResultType = ResultType.BadRequest;
                return result;
            }

            var newApprovalTemplate = new ApprovalTemplate()
            {
                isDeleted = false,
                CompanyId = request.ApprovalTemplateCreate.CompanyId,
                TemplateName = request.ApprovalTemplateCreate.TemplateName,
                CreatedById = request.UserId
            };

            _dbcontext.ApprovalTemplates.Add(newApprovalTemplate);
            _dbcontext.SaveChanges();

            result.Entity = _mapper.Map<IEnumerable<ApprovalTemplateDto>>(GetApprovalTemplates(request.ApprovalTemplateCreate.CompanyId.Value));
            result.ResultType = ResultType.Success;
            return result;
        }
        public override Exception OnException(Exception ex)
        {
            return new Exception(ex.Message, ex);
        }

        public IEnumerable<ApprovalTemplate> GetApprovalTemplates(int companyId)
        {
            return _dbcontext.ApprovalTemplates.Where(x => x.CompanyId == companyId && !x.isDeleted).Include(x => x.ApprovalTemplateLevels).ThenInclude(x => x.Approvers).ToList();
        }
    }
}
