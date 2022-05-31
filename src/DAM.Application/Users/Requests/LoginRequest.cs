using DAM.Application.Users.Dtos;
using MediatR;

namespace DAM.Application.Users.Requests
{
    public class LoginRequest : IRequest<HandlerResult<LoginDto>>
    {
        public LoginDto LoginDto { get; private set; }

        public LoginRequest(LoginDto loginDto)
        {
            LoginDto = loginDto;
        }
    }
}

