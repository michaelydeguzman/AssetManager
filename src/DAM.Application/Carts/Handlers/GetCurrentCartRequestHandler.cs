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
    public class GetCurrentCartRequestHandler : HandlerBase<GetCurrentCartRequest, HandlerResult<CartDto>>
    {
        private readonly IDbContext _dbcontext;
        private readonly IMapper _mapper;

        public GetCurrentCartRequestHandler(IMapper mapper, IDbContext dbcontext)
        {
            _dbcontext = dbcontext ?? throw new ArgumentNullException(nameof(dbcontext));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        public override HandlerResult<CartDto> HandleRequest(GetCurrentCartRequest request, CancellationToken cancellationToken, HandlerResult<CartDto> result)
        {
            ApplicationUser user = _dbcontext.AppUsers.Where(u => u.Id == request.UserId).First();

            //Cart cart = null;

            //var carts = _dbcontext.Cart.Include(c => c.CartItems).Where(c => c.UserId == request.UserId).Where(c => c.IsCurrentCart == true);

            //if (carts.Any())
            //{
            //    cart = carts.First();
            //}
            //else
            //{
            //    var newCart = new Cart
            //    {
            //        UserId = request.UserId,
            //        IsCurrentCart = true
            //    };

            //    _dbcontext.Add(newCart);
            //    _dbcontext.SaveChanges();

            //    cart = newCart;
            //}
            var cart = _dbcontext.Cart.Include(c => c.CartItems).Where(c => c.UserId == request.UserId).Where(c => c.IsCurrentCart == true).FirstOrDefault();

            if(cart != null)
            {
                result.Entity = _mapper.Map<CartDto>(cart);
                result.ResultType = ResultType.Success;
            }
            else
            {
                result.ResultType = ResultType.NoData;
            }
           

            return result;
        }
    }
}
