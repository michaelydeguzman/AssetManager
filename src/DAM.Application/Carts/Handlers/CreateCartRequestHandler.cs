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

namespace DAM.Application.Carts.Handlers
{
    public class CreateCartRequestHandler : HandlerBase<CreateCartRequest, HandlerResult<CartDto>>
    {
        private readonly IDbContext _dbcontext;
        private readonly IMapper _mapper;

        public CreateCartRequestHandler(IMapper mapper, IDbContext dbcontext)
        {
            _dbcontext = dbcontext ?? throw new ArgumentNullException(nameof(dbcontext));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        public override HandlerResult<CartDto> HandleRequest(CreateCartRequest request, CancellationToken cancellationToken, HandlerResult<CartDto> result)
        {
            var cart = _mapper.Map<Cart>(request.CartDto);
            cart.UserId = request.UserID;

            if (cart.Id == 0)
            {
                _dbcontext.Cart.Add(cart);
                _dbcontext.SaveChanges();

                foreach (int assetID in request.CartDto.AssetIds)
                {
                    CartItem CartItem = new CartItem();

                    CartItem.AssetID = assetID;
                    CartItem.CartID = cart.Id;

                    _dbcontext.CartItems.Add(CartItem);
                }
            }
            else
            {
                _dbcontext.Cart.Update(cart);
                var currentItems = _dbcontext.CartItems.Where(c => c.CartID == request.CartDto.Id).ToList();
                var currentItemIDs = _dbcontext.CartItems.Where(c => c.CartID == request.CartDto.Id).Select(c => c.AssetID).ToList();

                foreach (int assetID in request.CartDto.AssetIds)
                {
                    if(!currentItemIDs.Contains(assetID))
                    {
                        CartItem CartItem = new CartItem();

                        CartItem.AssetID = assetID;
                        CartItem.CartID = cart.Id;

                        _dbcontext.CartItems.Add(CartItem);
                    }
                }

                foreach (CartItem cartItem in currentItems)
                {
                    if (!request.CartDto.AssetIds.Contains(cartItem.AssetID))
                    {
                        _dbcontext.CartItems.Remove(cartItem);
                    }
                }

            }
            
            _dbcontext.SaveChanges();
            result.ResultType = ResultType.Success;

            return result;
        }
    }
}
