using DAM.Application.Emails.Dtos;
using MediatR;

namespace DAM.Application.Emails.Requests
{
    public class SaveEmailTemplateRequest : IRequest<HandlerResult<EmailTemplateDto>>
    {
        public string UserId { get; set; }

        public EmailTemplateDto EmailTemplate { get; set; }

        public SaveEmailTemplateRequest(EmailTemplateDto email, string userId)
        {
            UserId = userId;
            EmailTemplate = email;
        }
    }
}
