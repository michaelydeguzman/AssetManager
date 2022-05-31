using System;
using System.Threading;
using AutoMapper;
using DAM.Application.Carts.Dtos;
using DAM.Application.Carts.Requests;
using DAM.Persistence;
using DAM.Domain.Entities;
using System.Collections.Generic;
using DAM.Application.Assets.Dtos;
using System.Linq;
using DAM.Domain.Entities.Identity;
using Microsoft.EntityFrameworkCore;

namespace DAM.Application.Carts.Handlers
{
    public class DeleteCartRequestHandler : HandlerBase<DeleteCartRequest, HandlerResult<CartDto>>
    {
        private readonly IDbContext _dbcontext;
        private readonly IMapper _mapper;

        public DeleteCartRequestHandler(IMapper mapper, IDbContext dbcontext)
        {
            _dbcontext = dbcontext ?? throw new ArgumentNullException(nameof(dbcontext));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        public override HandlerResult<CartDto> HandleRequest(DeleteCartRequest request, CancellationToken cancellationToken, HandlerResult<CartDto> result)
        {
            var cart = _dbcontext.Cart.Include(c => c.CartItems).Where(c => c.Id == request.CartDto.Id).FirstOrDefault();

            if(cart == null)
            {
                result.ResultType = ResultType.BadRequest;
                return result;
            }

            foreach (CartItem cartItem in cart.CartItems)
            {
                _dbcontext.CartItems.Remove(cartItem);
            }

            _dbcontext.Cart.Remove(cart);

            _dbcontext.SaveChanges();

            result.ResultType = ResultType.Success;
            return result;
        }
    }
}
