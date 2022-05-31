using AutoMapper;
using DAM.Application.Users.Dtos;
using DAM.Domain.Entities;
using DAM.Persistence;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using static DAM.Application.Users.Requests.UserRequest;

namespace DAM.Application.Users.Handlers
{
    public class GetUserOOORequestHandler : HandlerBase<GetUserOOORequest, HandlerResult<IEnumerable<UserOOODto>>>
    {

        private readonly IDbContext _dbcontext;
        private readonly IMapper _mapper;

        public GetUserOOORequestHandler(IDbContext dbcontext, IMapper mapper)
        {
            _dbcontext = dbcontext ?? throw new ArgumentNullException(nameof(dbcontext));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        public override HandlerResult<IEnumerable<UserOOODto>> HandleRequest(GetUserOOORequest request, CancellationToken cancellationToken, HandlerResult<IEnumerable<UserOOODto>> result)
        {
            var userOOO = new List<UserOOO>();

            if (string.IsNullOrEmpty(request.UserId))
            {
                userOOO = _dbcontext.UserOOO.Where(o => !o.Deleted).ToList();
            }
            else
            {
                userOOO = _dbcontext.UserOOO.Where(o => o.UserId == request.UserId && !o.Deleted).ToList();
            }

            result.Entity = _mapper.Map<IEnumerable<UserOOODto>>(userOOO);

            return result;
        }
    }
}
