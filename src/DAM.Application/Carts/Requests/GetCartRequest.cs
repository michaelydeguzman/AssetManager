using System;
using System.Collections.Generic;
using DAM.Application.Carts.Dtos;
using MediatR;

namespace DAM.Application.Carts.Requests
{
    public class GetCartRequest : IRequest<HandlerResult<CartDto>>
    {
        public int CartID { get; private set; }

        public GetCartRequest(int cartID)
        {
            CartID = cartID;
        }
    }
}
