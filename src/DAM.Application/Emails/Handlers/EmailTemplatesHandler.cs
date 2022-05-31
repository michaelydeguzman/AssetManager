using AutoMapper;
using DAM.Application.Cache;
using DAM.Persistence;
using System;
using System.Collections.Generic;
using System.Threading;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using DAM.Application.Emails.Requests;
using DAM.Application.Emails.Dtos;
using DAM.Application.Services.Interfaces;

namespace DAM.Application.Teams.Handlers
{
    public class EmailTemplatesRequestHandler : HandlerBase<EmailTemplatesRequest, HandlerResult<IEnumerable<EmailTemplateDto>>>
    {
        private readonly IDbContext _dbcontext;
        private readonly IMapper _mapper;
        private readonly IEmailService _emailService;

        public EmailTemplatesRequestHandler(ICacheProvider cacheProvider, IDbContext dbcontext, IMapper mapper, IEmailService emailService)
        {
            _dbcontext = dbcontext ?? throw new ArgumentNullException(nameof(dbcontext));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _emailService = emailService ?? throw new ArgumentNullException(nameof(emailService));
        }

        public override HandlerResult<IEnumerable<EmailTemplateDto>> HandleRequest(EmailTemplatesRequest request, CancellationToken cancellationToken, HandlerResult<IEnumerable<EmailTemplateDto>> result)
        {
            var emailTemplates = _dbcontext.EmailTemplates.Where(t=>!t.Deleted);

            result.Entity = _mapper.Map<List<EmailTemplateDto>>(emailTemplates);

            foreach (var template in result.Entity)
            {
                var headerUrl = _emailService.GetEmailHeaderUrl(template.Classification);

                template.Contents = template.Contents.Replace("%%EmailHeader%%", headerUrl);
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