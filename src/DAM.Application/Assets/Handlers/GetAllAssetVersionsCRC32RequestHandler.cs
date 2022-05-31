using AutoMapper;
using Azure.Storage.Blobs;
using DAM.Application.Assets.Requests;
using DAM.Persistence;
using System;
using System.Linq;
using System.Threading;
using Azure.Storage;
using Microsoft.Extensions.Configuration;
using DAM.Application.Services.Interfaces;
using Force.Crc32;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

namespace DAM.Application.Assets.Handlers
{
    public class GetAllAssetVersionsCRC32RequestHandler : HandlerBase<GetAllAssetVersionsCRC32Request, HandlerResult<string>>
    {
        private readonly IDbContext _dbcontext;
        private readonly IMapper _mapper;
        private readonly IConfiguration _configuration;
        private readonly IAzureStorageService _azureStorageService;

        public GetAllAssetVersionsCRC32RequestHandler(IDbContext dbcontext, IMapper mapper, IConfiguration configuration, IAzureStorageService azureStorageService)
        {
            _dbcontext = dbcontext ?? throw new ArgumentNullException(nameof(dbcontext));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
            _azureStorageService = azureStorageService ?? throw new ArgumentNullException(nameof(azureStorageService));
        }

        public override HandlerResult<string> HandleRequest(GetAllAssetVersionsCRC32Request request, CancellationToken cancellationToken, HandlerResult<string> result)
        {
            var allVersions = _dbcontext.AssetVersions.ToList();
            result.Entity = "";

            // Update Asset URLs
            foreach (var version in allVersions)
            {               
                try
                {
                    var contentType = "";
                    var data = _azureStorageService.GetBlobStream(_configuration, version.Key + "." + version.Extension, out contentType);
                    byte[] bytes = new byte[data.Length];
                    uint crc = Crc32Algorithm.Compute(bytes);
                    version.CRC32Code = crc;
                    _dbcontext.AssetVersions.Update(version);
                }
                catch (Exception ex) 
                { }
                _dbcontext.SaveChanges();
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