using AutoMapper;
using DAM.Application.Cache;
using DAM.Application.Services.Interfaces;
using DAM.Application.Users.Dtos;
using DAM.Domain.Entities;
using DAM.Domain.Entities.Identity;
using DAM.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using static DAM.Application.Users.Requests.UserRequest;

namespace DAM.Application.Users.Handlers
{
    public class GetUsersRequestHandler : HandlerBase<GetUsersRequest, HandlerResult<IEnumerable<UserDto>>>
    {
        private readonly ICacheProvider _cacheProvider;
        private readonly IDbContext _dbcontext;
        private readonly IMapper _mapper;
        private readonly IConfiguration _configuration;
        private readonly IAzureStorageService _azureStorageService;

        public GetUsersRequestHandler(ICacheProvider cacheProvider, IDbContext dbcontext, IMapper mapper, IConfiguration configuration, IAzureStorageService azureStorageService)
        {
            _cacheProvider = cacheProvider ?? throw new ArgumentNullException(nameof(cacheProvider));
            _dbcontext = dbcontext ?? throw new ArgumentNullException(nameof(dbcontext));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
            _azureStorageService = azureStorageService ?? throw new ArgumentNullException(nameof(azureStorageService));
        }

        public override HandlerResult<IEnumerable<UserDto>> HandleRequest(GetUsersRequest request, CancellationToken cancellationToken, HandlerResult<IEnumerable<UserDto>> result)
        {
            List<ApplicationUser> users = new List<ApplicationUser>();
            var allUserRole = _dbcontext.UserRoles.ToList();
            var allUserFolders = _dbcontext.UserFolders.ToList();
            var allFolders = _dbcontext.Folders.ToList();
            var allCompanies = _dbcontext.Companies.ToList();
            if (string.IsNullOrEmpty(request.UserId) || request.UserId == "0")
            {
                users = _dbcontext.AppUsers.ToList();
            }
            else
            {
                users = _dbcontext.AppUsers.Where(a => a.Id == request.UserId).ToList();
            }

            result.Entity = _mapper.Map<IEnumerable<UserDto>>(users);

            foreach (var user in result.Entity)
            {
                if (!string.IsNullOrEmpty(user.ImageFileExtension))
                {
                    user.ImageUrl = _azureStorageService.GetBlobSasUri(_configuration, string.Concat(user.Id, ".", user.ImageFileExtension));
                }

                user.UserRole = allUserRole.FirstOrDefault(r => r.Id == user.UserRoleId);
                user.UserFolders = allUserFolders.Where(uf => uf.UserId == user.Id).ToList();
                if(user.UserFolders !=null && user.UserFolders.Count > 0)
                {
                    foreach (var folder in user.UserFolders)
                    {
                        folder.Folder = allFolders
                            .Where(f => f.Id == folder.Id)
                            .Select(f=> new Folder
                            {
                                Id = f.Id,
                                FolderName = f.FolderName, 
                                Description = f.Description,
                                ParentFolderId = f.ParentFolderId,
                                Deleted = f.Deleted
                            })
                            .FirstOrDefault();
                    }
                }

                user.CompanyName = user.CompanyId.HasValue ? allCompanies.FirstOrDefault(x => x.Id == user.CompanyId.Value).CompanyName : "";
            }
            result.ResultType = ResultType.Success;
            return result;
        }

        public override Exception OnException(Exception ex)
        {
            return new Exception(ex.Message, ex);
        }
    }
}
