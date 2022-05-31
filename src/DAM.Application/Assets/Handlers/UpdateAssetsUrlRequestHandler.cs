using AutoMapper;
using DAM.Application.Assets.Requests;
using DAM.Application.Services.Interfaces;
using DAM.Persistence;
using Microsoft.Extensions.Configuration;
using System;
using System.Threading;

namespace DAM.Application.Assets.Handlers
{
    public class UpdateAssetsUrlRequestHandler : HandlerBase<UpdateAssetsUrlRequest, HandlerResult<string>>
    {
        private readonly IDbContext _dbcontext;
        private readonly IMapper _mapper;
        private readonly IConfiguration _configuration;
        private readonly IAzureStorageService _azureStorageService;
        private readonly IConversionService _conversionService;
        private readonly ITagService _tagService;
        private readonly IHelperService _helperService;


        public UpdateAssetsUrlRequestHandler(IDbContext dbcontext, IMapper mapper, IConfiguration configuration, IAzureStorageService azureStorageService,
                IConversionService conversionService, ITagService tagService, IHelperService helperService)
        {
            _dbcontext = dbcontext ?? throw new ArgumentNullException(nameof(dbcontext));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
            _azureStorageService = azureStorageService ?? throw new ArgumentNullException(nameof(azureStorageService));
            _conversionService = conversionService ?? throw new ArgumentNullException(nameof(conversionService));
            _tagService = tagService ?? throw new ArgumentNullException(nameof(tagService));
            _helperService = helperService ?? throw new ArgumentNullException(nameof(helperService));
        }

        public override HandlerResult<string> HandleRequest(UpdateAssetsUrlRequest request, CancellationToken cancellationToken, HandlerResult<string> result)
        {
            var assetsVersions = _dbcontext.AssetVersions;
            foreach (var version in assetsVersions)
            {
                version.DownloadUrl = _azureStorageService.GetBlobSasUri(_configuration, string.Concat(version.Key, ".", version.Extension), version.FileName, string.Empty, false, true);
                if (version.Thumbnail.ToLower().IndexOf("http") >= 0)
                { 
                    version.Thumbnail = _azureStorageService.GetBlobSasUri(_configuration, string.Concat(version.Key, ".", version.Extension), string.Empty,string.Empty, true);
                }
                version.OriginalUrl = _azureStorageService.GetBlobSasUri(_configuration, string.Concat(version.Key, ".", version.Extension), string.Empty, version.FileType == "" ? "text/plain" : version.FileType, false);
                _dbcontext.AssetVersions.Update(version);
            }
            _dbcontext.SaveChanges();

            result.Entity = "done";
            result.ResultType = ResultType.Success;
            return result;
        }
    }
}
