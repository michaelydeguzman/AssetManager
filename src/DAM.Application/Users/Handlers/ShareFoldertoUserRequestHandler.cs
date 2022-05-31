using AutoMapper;
using DAM.Application.Cache;
using DAM.Application.Folders.Dtos;
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
    public class ShareFoldertoUserRequestHandler : HandlerBase<ShareFoldertoUserRequest, HandlerResult<UserDto>>
    {
        private readonly ICacheProvider _cacheProvider;
        private readonly IDbContext _dbcontext;
        private readonly IMapper _mapper;

        public ShareFoldertoUserRequestHandler(ICacheProvider cacheProvider, IDbContext dbcontext, IMapper mapper)
        {
            _cacheProvider = cacheProvider ?? throw new ArgumentNullException(nameof(cacheProvider));
            _dbcontext = dbcontext ?? throw new ArgumentNullException(nameof(dbcontext));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }
        public override HandlerResult<UserDto> HandleRequest(ShareFoldertoUserRequest request, CancellationToken cancellationToken, HandlerResult<UserDto> result)
        {
            int folderId = 0;
            int.TryParse(request.UserDto.FolderId, out folderId);
            
            if (request.UserDto.UserIds.Count > 0 && folderId > 0)
            {
                foreach(var userId in request.UserDto.UserIds)
                {
                    // delete old folders
                    var prevUserFolders = _dbcontext.UserFolders.Where(up => up.UserId == userId).ToList();
                    _dbcontext.UserFolders.RemoveRange(prevUserFolders);

                    var foldersToAccess = new List<UserFolder>();
                    prevUserFolders.ForEach(folder =>
                    {
                        var f = new UserFolder()
                        {
                            FolderId = folder.FolderId,
                            UserId = folder.UserId
                        };
                        foldersToAccess.Add(f);
                    });
                    var f = new UserFolder()
                    {
                        FolderId = folderId,
                        UserId = userId
                    };
                    foldersToAccess.Add(f);
                    var newUserFolders = foldersToAccess.Distinct().ToList();

                    if (newUserFolders.Count > 0)
                    {
                        _dbcontext.UserFolders.AddRange(newUserFolders);

                    }
                    _dbcontext.SaveChanges();
                }
            }
            result.Entity = request.UserDto;
            result.ResultType = ResultType.Success;
            return result;
        }
        public override Exception OnException(Exception ex)
        {
            return new Exception(ex.Message, ex);
        }
    }
}
