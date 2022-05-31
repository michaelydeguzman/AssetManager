using AutoMapper;
using DAM.Application.Cache;
using DAM.Application.UserRoles.Dtos;
using DAM.Domain.Entities;
using DAM.Persistence;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using static DAM.Application.UserRoles.Requests.UserRolesRequest;

namespace DAM.Application.UserRoles.Handlers
{
    public class GetAllUserRolesRequestHandler : HandlerBase<GetAllUserRolesRequest, HandlerResult<IEnumerable<UserRoleDto>>>
    {
        private readonly IDbContext _dbcontext;
        private readonly IMapper _mapper;

        public GetAllUserRolesRequestHandler( IDbContext dbcontext, IMapper mapper)
        {
            _dbcontext = dbcontext ?? throw new ArgumentNullException(nameof(dbcontext));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        public override HandlerResult<IEnumerable<UserRoleDto>> HandleRequest(GetAllUserRolesRequest request, CancellationToken cancellationToken, HandlerResult<IEnumerable<UserRoleDto>> result)
        {
            var userRoles = _dbcontext.UserRoles.ToList();

            result.Entity = _mapper.Map<IEnumerable<UserRoleDto>>(userRoles);
            result.ResultType = ResultType.Success;
            return result;
        }

        public override Exception OnException(Exception ex)
        {
            return new Exception(ex.Message, ex);
        }
    }
}
