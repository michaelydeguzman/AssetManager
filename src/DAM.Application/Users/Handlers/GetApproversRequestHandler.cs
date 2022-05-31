using AutoMapper;
using DAM.Application.Cache;
using DAM.Application.Users.Dtos;
using DAM.Domain.Entities;
using DAM.Domain.Entities.Identity;
using DAM.Persistence;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using static DAM.Application.Users.Requests.UserRequest;

namespace DAM.Application.Users.Handlers
{
    public class GetApproversRequestHandler : HandlerBase<GetApproversRequest, HandlerResult<IEnumerable<UserDto>>>
    {
        private readonly ICacheProvider _cacheProvider;
        private readonly IDbContext _dbcontext;
        private readonly IMapper _mapper;
        private readonly IConfiguration _configuration;

        public GetApproversRequestHandler(ICacheProvider cacheProvider, IDbContext dbcontext, IMapper mapper, IConfiguration configuration)
        {
            _cacheProvider = cacheProvider ?? throw new ArgumentNullException(nameof(cacheProvider));
            _dbcontext = dbcontext ?? throw new ArgumentNullException(nameof(dbcontext));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
        }

        public override HandlerResult<IEnumerable<UserDto>> HandleRequest(GetApproversRequest request, CancellationToken cancellationToken, HandlerResult<IEnumerable<UserDto>> result)
        {
            List<ApplicationUser> users = new List<ApplicationUser>();

            users = _dbcontext.AppUsers.ToList();
            var userRoles = _dbcontext.UserRoles.ToList();

            var approvers = from u in users
                            join ur in userRoles
                            on u.UserRoleId equals ur.Id
                            where ur.CanApprove == true
                            select u;

            result.Entity = _mapper.Map<IEnumerable<UserDto>>(approvers);
            result.ResultType = ResultType.Success;
            return result;
        }

        public override Exception OnException(Exception ex)
        {
            return new Exception(ex.Message, ex);
        }
    }
}
