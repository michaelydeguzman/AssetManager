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
    public class AddUserOOORequestHandler : HandlerBase<AddUserOOORequest, HandlerResult<UserOOODto>>
    {

        private readonly IDbContext _dbcontext;
        private readonly IMapper _mapper;

        public AddUserOOORequestHandler(IDbContext dbcontext, IMapper mapper)
        {
            _dbcontext = dbcontext ?? throw new ArgumentNullException(nameof(dbcontext));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        public override HandlerResult<UserOOODto> HandleRequest(AddUserOOORequest request, CancellationToken cancellationToken, HandlerResult<UserOOODto> result)
        {
            var newUserOOO = _mapper.Map<UserOOO>(request.UserOOO);

            var userAllOOO = _dbcontext.UserOOO.Where(x => x.UserId == newUserOOO.UserId && !x.Deleted).ToList();

            var doesRangeExist = userAllOOO.Any(x => x.StartDate <= newUserOOO.EndDate && x.EndDate >= newUserOOO.StartDate);

            if (doesRangeExist)
            {
                result.Entity = _mapper.Map<UserOOODto>(newUserOOO);
                result.ResultType = ResultType.Fail;
                result.Message = "Overlap with existing ooo found.";

                return result;
            }

            _dbcontext.UserOOO.Add(newUserOOO);
            _dbcontext.SaveChanges();

            result.Entity = _mapper.Map<UserOOODto>(newUserOOO);
            result.ResultType = ResultType.Success;

            return result;
        }
    }
}
