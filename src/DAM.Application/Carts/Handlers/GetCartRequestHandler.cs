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
using Microsoft.EntityFrameworkCore;

namespace DAM.Application.Carts.Handlers
{
    public class GetCartRequestHandler : HandlerBase<GetCartRequest, HandlerResult<CartDto>>
    {
        private readonly IDbContext _dbcontext;
        private readonly IMapper _mapper;

        public GetCartRequestHandler(IMapper mapper, IDbContext dbcontext)
        {
            _dbcontext = dbcontext ?? throw new ArgumentNullException(nameof(dbcontext));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        public override HandlerResult<CartDto> HandleRequest(GetCartRequest request, CancellationToken cancellationToken, HandlerResult<CartDto> result)
        {
            var cart = _dbcontext.Cart.Include(c => c.CartItems).Where(up => up.Id == request.CartID).FirstOrDefault();

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
