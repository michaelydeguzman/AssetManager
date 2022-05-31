using AutoMapper;
using DAM.Application.Users.Dtos;
using DAM.Persistence;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using static DAM.Application.Users.Requests.UserRequest;

namespace DAM.Application.Users.Handlers
{
    public class GetUserFolderRequestHandler : HandlerBase<GetUserFolderRequest, HandlerResult<IEnumerable<UserFolderDto>>>
    {

        private readonly IDbContext _dbcontext;
        private readonly IMapper _mapper;

        public GetUserFolderRequestHandler(IDbContext dbcontext, IMapper mapper)
        {
            _dbcontext = dbcontext ?? throw new ArgumentNullException(nameof(dbcontext));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }
        public override HandlerResult<IEnumerable<UserFolderDto>> HandleRequest(GetUserFolderRequest request, CancellationToken cancellationToken, HandlerResult<IEnumerable<UserFolderDto>> result)
        {
            var userFolders = _dbcontext.UserFolders.Where(up => up.UserId == request.UserId).ToList();

            result.Entity = _mapper.Map<IEnumerable<UserFolderDto>>(userFolders);

            return result;
        }

    }
}
