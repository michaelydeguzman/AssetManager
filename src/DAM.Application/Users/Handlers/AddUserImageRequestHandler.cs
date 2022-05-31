using AutoMapper;
using Azure.Storage.Blobs.Models;
using DAM.Application.Cache;
using DAM.Application.Services.Interfaces;
using DAM.Application.Users.Dtos;
using DAM.Persistence;
using Microsoft.Extensions.Configuration;
using System;
using System.IO;
using System.Linq;
using System.Threading;
using static DAM.Application.Users.Requests.UserRequest;

namespace DAM.Application.Users.Handlers
{
    public class AddUserImageRequestHandler : HandlerBase<AddUserImageRequest, HandlerResult<UserDto>>
    {
        private readonly ICacheProvider _cacheProvider;
        private readonly IDbContext _dbcontext;
        private readonly IMapper _mapper;
        private readonly IConfiguration _configuration;
        private readonly IAzureStorageService _azureStorageService;

        public AddUserImageRequestHandler(ICacheProvider cacheProvider, IDbContext dbcontext, IMapper mapper, IConfiguration configuration, IAzureStorageService azureStorageService)
        {
            _cacheProvider = cacheProvider ?? throw new ArgumentNullException(nameof(cacheProvider));
            _dbcontext = dbcontext ?? throw new ArgumentNullException(nameof(dbcontext));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
            _azureStorageService = azureStorageService ?? throw new ArgumentNullException(nameof(azureStorageService));
        }

        public override HandlerResult<UserDto> HandleRequest(AddUserImageRequest request, CancellationToken cancellationToken, HandlerResult<UserDto> result)
        {
            var user = _mapper.Map<UserDto>(request.UserDto);
            var userDetails = _dbcontext.AppUsers.Where(x => x.Id == user.Id).FirstOrDefault();
            using (Stream stream = new MemoryStream(request.UserDto.FileBytes))
            {
                BlobProperties userProps = _azureStorageService.UploadFileToStorage(_configuration, stream, string.Concat(userDetails.Id, ".", request.UserDto.ImageFileExtension), out Uri assetUri, false, true);
                userDetails.ImageFileExtension = request.UserDto.ImageFileExtension;
            }

            _dbcontext.SaveChanges();

            user.ImageUrl = _azureStorageService.GetBlobSasUri(_configuration, string.Concat(userDetails.Id,".", userDetails.ImageFileExtension));
            result.Entity = user;
            result.ResultType = ResultType.Success;
            return result;
        }   
    }
}
