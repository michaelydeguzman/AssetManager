using System;
using System.Collections.Generic;
using DAM.Application.Carts.Dtos;
using MediatR;

namespace DAM.Application.Carts.Requests
{
    public class GetCartsRequest : IRequest<HandlerResult<IEnumerable<CartDto>>>
    {
        public string UserId { get; private set; }

        public GetCartsRequest(String userId)
        {
            UserId = userId;
        } 
    }
}
