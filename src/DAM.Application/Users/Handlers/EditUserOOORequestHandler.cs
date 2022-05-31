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
    public class EditUserOOORequestHandler : HandlerBase<EditUserOOORequest, HandlerResult<UserOOODto>>
    {

        private readonly IDbContext _dbcontext;
        private readonly IMapper _mapper;

        public EditUserOOORequestHandler(IDbContext dbcontext, IMapper mapper)
        {
            _dbcontext = dbcontext ?? throw new ArgumentNullException(nameof(dbcontext));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        public override HandlerResult<UserOOODto> HandleRequest(EditUserOOORequest request, CancellationToken cancellationToken, HandlerResult<UserOOODto> result)
        {
            var editUserOOO = _mapper.Map<UserOOO>(request.UserOOO);
            var exists = _dbcontext.UserOOO.Any(x => x.Id == editUserOOO.Id);

            if (!exists)
            {
                result.Entity = _mapper.Map<UserOOODto>(editUserOOO);
                result.ResultType = ResultType.NoData;
                result.Message = "OOO not found.";

                return result;
            }

            _dbcontext.UserOOO.Update(editUserOOO);
            _dbcontext.SaveChanges();

            result.Entity = _mapper.Map<UserOOODto>(editUserOOO);
            result.ResultType = ResultType.Success;

            return result;
        }
    }
}
