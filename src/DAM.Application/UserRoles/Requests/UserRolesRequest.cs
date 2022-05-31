using DAM.Application.UserRoles.Dtos;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace DAM.Application.UserRoles.Requests
{
    public class UserRolesRequest : IRequest<HandlerResult<IEnumerable<UserRoleDto>>>
    {
        public class GetAllUserRolesRequest : IRequest<HandlerResult<IEnumerable<UserRoleDto>>>
        {
            public GetAllUserRolesRequest()
            {
            }
        }
    }
}