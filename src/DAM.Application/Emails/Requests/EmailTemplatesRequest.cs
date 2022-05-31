using DAM.Application.Emails.Dtos;
using MediatR;
using System.Collections.Generic;

namespace DAM.Application.Emails.Requests
{
    public class EmailTemplatesRequest : IRequest<HandlerResult<IEnumerable<EmailTemplateDto>>>
    {
        public int EmailTemplateId { get; set; }
        public EmailTemplatesRequest(int emailTemplateId)
        {
            EmailTemplateId = emailTemplateId;
        }
    }
}
