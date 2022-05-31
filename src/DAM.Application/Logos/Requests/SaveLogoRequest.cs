using DAM.Application.Logos.Dtos;
using MediatR;

namespace DAM.Application.Logos.Requests
{
    public class SaveLogoRequest : IRequest<HandlerResult<LogoDto>>
    {
        public string UserId { get; set; }

        public LogoDto Logo { get; set; }

        public SaveLogoRequest(LogoDto logo, string userId)
        {
            UserId = userId;
            Logo = logo;
        }
    }
}
