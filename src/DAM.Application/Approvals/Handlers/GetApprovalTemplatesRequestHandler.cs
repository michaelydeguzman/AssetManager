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
    public class GetApprovalTemplatesRequestHandler : HandlerBase<GetApprovalTemplatesRequest, HandlerResult<IEnumerable<ApprovalTemplateDto>>>
    {
        private readonly ICacheProvider _cacheProvider;
        private readonly IDbContext _dbcontext;
        private readonly IMapper _mapper;
        private readonly IConfiguration _configuration;

        public GetApprovalTemplatesRequestHandler(ICacheProvider cacheProvider, IDbContext dbcontext, IMapper mapper, IConfiguration configuration)
        {
            _cacheProvider = cacheProvider ?? throw new ArgumentNullException(nameof(cacheProvider));
            _dbcontext = dbcontext ?? throw new ArgumentNullException(nameof(dbcontext));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
        }

        public override HandlerResult<IEnumerable<ApprovalTemplateDto>> HandleRequest(GetApprovalTemplatesRequest request, CancellationToken cancellationToken, 
            HandlerResult<IEnumerable<ApprovalTemplateDto>> result)
        {
            var approvalTemplates = new List<ApprovalTemplate>();

            if (request.CompanyId == 0)
            {
                approvalTemplates = _dbcontext.ApprovalTemplates.Where(x=>!x.isDeleted).Include(x => x.ApprovalTemplateLevels).ThenInclude(x => x.Approvers).ToList();
            } 
            else
            {
                approvalTemplates = _dbcontext.ApprovalTemplates.Where(x => x.CompanyId == request.CompanyId && !x.isDeleted).Include(x => x.ApprovalTemplateLevels).ThenInclude(x => x.Approvers).ToList();
            }

            result.Entity = _mapper.Map<IEnumerable<ApprovalTemplateDto>>(approvalTemplates);
            result.ResultType = ResultType.Success;
    
            return result;
        }

        public override Exception OnException(Exception ex)
        {
            return new Exception(ex.Message, ex);
        }
    }
}
