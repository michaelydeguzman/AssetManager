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
    public class GetCartsRequestHandler : HandlerBase<GetCartsRequest, HandlerResult<IEnumerable<CartDto>>>
    {
        private readonly IDbContext _dbcontext;
        private readonly IMapper _mapper;

        public GetCartsRequestHandler(IMapper mapper, IDbContext dbcontext)
        {
            _dbcontext = dbcontext ?? throw new ArgumentNullException(nameof(dbcontext));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        public override HandlerResult<IEnumerable<CartDto>> HandleRequest(GetCartsRequest request, CancellationToken cancellationToken, HandlerResult<IEnumerable<CartDto>> result)
        {
            var carts = _dbcontext.Cart.Include(c => c.CartItems).Where(up => up.UserId == request.UserId).ToList();

            result.Entity = _mapper.Map<IEnumerable<CartDto>>(carts);

            result.ResultType = ResultType.Success;
            return result;
        }
    }
}
