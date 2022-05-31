using AutoMapper;
using DAM.Application.Cache;
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
    public class AddUserRequestHandler : HandlerBase<AddUserRequest, HandlerResult<UserDto>>
    {
        private readonly ICacheProvider _cacheProvider;
        private readonly IDbContext _dbcontext;
        private readonly IMapper _mapper;

        public AddUserRequestHandler(ICacheProvider cacheProvider, IDbContext dbcontext, IMapper mapper)
        {
            _cacheProvider = cacheProvider ?? throw new ArgumentNullException(nameof(cacheProvider));
            _dbcontext = dbcontext ?? throw new ArgumentNullException(nameof(dbcontext));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }
        public override HandlerResult<UserDto> HandleRequest(AddUserRequest request, CancellationToken cancellationToken, HandlerResult<UserDto> result)
        {
            //var user = _mapper.Map<User>(request.UserDto);
            //var existingUser = _dbcontext.Users.Where(u => u.EmailAddress == user.EmailAddress && u.Active).FirstOrDefault();
            //if(existingUser== null)
            //{
            //    _dbcontext.Users.Add(user);
            //    _dbcontext.SaveChanges();
            //}
            //result.Entity = _mapper.Map<UserDto>(_dbcontext.Users.First(a => a.Id == user.Id));
            //result.ResultType = ResultType.Success;
            return result;
        }
        public override Exception OnException(Exception ex)
        {
            return new Exception(ex.Message, ex);
        }
    }
}
