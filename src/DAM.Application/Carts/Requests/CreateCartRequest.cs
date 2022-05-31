using System;
using DAM.Application.Carts.Dtos;
using MediatR;

namespace DAM.Application.Carts.Requests
{
    public class CreateCartRequest : IRequest<HandlerResult<CartDto>>
    {
       public CartDto CartDto { get; private set; }

       public string UserID { get; private set; }

       public CreateCartRequest(CartDto cartDto, String userID)
       {
            CartDto = cartDto;
            UserID = userID;
       }
    }

   
}
