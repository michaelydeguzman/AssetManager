using AutoMapper;
using DAM.Application.Cache;
using DAM.Persistence;
using System;
using System.Linq;
using System.Threading;
using Microsoft.Extensions.Configuration;
using DAM.Domain.Entities;
using DAM.Application.Emails.Dtos;
using DAM.Application.Emails.Requests;
using System.Collections.Generic;

namespace DAM.Application.Teams.Handlers
{
    public class SaveEmailTemplateRequestHandler : HandlerBase<SaveEmailTemplateRequest, HandlerResult<EmailTemplateDto>>
    {
        private readonly IDbContext _dbcontext;
        private readonly IMapper _mapper;
        private readonly IConfiguration _configuration;

        public SaveEmailTemplateRequestHandler(ICacheProvider cacheProvider, IDbContext dbcontext, IMapper mapper, IConfiguration configuration)
        {
            _dbcontext = dbcontext ?? throw new ArgumentNullException(nameof(dbcontext));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
        }

        public override HandlerResult<EmailTemplateDto> HandleRequest(SaveEmailTemplateRequest request, CancellationToken cancellationToken, HandlerResult<EmailTemplateDto> result)
        {
            result.Entity = new EmailTemplateDto();
            var emailTemplate = _dbcontext.EmailTemplates.FirstOrDefault(x => x.Id == request.EmailTemplate.Id);

            if (emailTemplate == null)
            {
                result.ResultType = ResultType.NoData;
                return result;
            }
            else
            {
                emailTemplate.Subject = request.EmailTemplate.Subject;
                emailTemplate.Message = request.EmailTemplate.Message;
             
                _dbcontext.EmailTemplates.Update(emailTemplate);
            }

            _dbcontext.SaveChanges();

            result.Entity = _mapper.Map<EmailTemplateDto>(emailTemplate);
            result.ResultType = ResultType.Success;

            return result;
        }

        public override Exception OnException(Exception ex)
        {
            return new Exception(ex.Message, ex);
        }
    }
}