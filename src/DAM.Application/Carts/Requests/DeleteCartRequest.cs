using System;
using System.Collections.Generic;
using DAM.Application.Carts.Dtos;
using MediatR;

namespace DAM.Application.Carts.Requests
{   
    public class DeleteCartRequest : IRequest<HandlerResult<CartDto>>
    {
        public CartDto CartDto { get; private set; }

        public DeleteCartRequest(CartDto cartDto)
        {
            CartDto = cartDto;
        }
    }   
}
