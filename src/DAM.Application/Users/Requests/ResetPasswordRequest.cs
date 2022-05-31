using DAM.Application.Users.Dtos;
using MediatR;

namespace DAM.Application.Users.Requests
{
    public class ResetPasswordRequest : IRequest<HandlerResult<LoginDto>>
    {
        public LoginDto LoginDto { get; private set; }

        public ResetPasswordRequest(LoginDto loginDto)
        {
            LoginDto = loginDto;
        }
    }
}

