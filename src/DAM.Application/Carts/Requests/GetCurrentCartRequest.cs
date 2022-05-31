using System;
using System.Collections.Generic;
using DAM.Application.Carts.Dtos;
using MediatR;

namespace DAM.Application.Carts.Requests
{
    public class GetCurrentCartRequest : IRequest<HandlerResult<CartDto>>
    {
        public string UserId { get; private set; }

        public GetCurrentCartRequest(String userId)
        {
            UserId = userId;
        }
    }
}
