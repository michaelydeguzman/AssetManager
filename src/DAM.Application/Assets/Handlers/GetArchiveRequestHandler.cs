using AutoMapper;
using Azure.Storage.Blobs;
using DAM.Application.Accounts.Dtos;
using DAM.Application.Assets.Dtos;
using DAM.Application.Assets.Requests;
using DAM.Application.Cache;
using DAM.Application.CountryRegions.Dtos;
using DAM.Domain.Entities;
using DAM.Persistence;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Microsoft.Extensions.Configuration;
using DAM.Application.Assets.Enums;
using DAM.Application.Services.Interfaces;

namespace DAM.Application.Assets.Handlers
{
    public class GetArchiveRequestHandler : HandlerBase<GetArchiveRequest, HandlerResult<IEnumerable<AssetDto>>>
    {
        private readonly ICacheProvider _cacheProvider;
        private readonly IDbContext _dbcontext;
        private readonly IMapper _mapper;
        private readonly IConfiguration _configuration;
        private readonly IAzureStorageService _azureStorageService;

        public GetArchiveRequestHandler(ICacheProvider cacheProvider, IDbContext dbcontext, IMapper mapper, IConfiguration configuration, IAzureStorageService azureStorageService)
        {
            _cacheProvider = cacheProvider ?? throw new ArgumentNullException(nameof(cacheProvider));
            _dbcontext = dbcontext ?? throw new ArgumentNullException(nameof(dbcontext));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
            _azureStorageService = azureStorageService ?? throw new ArgumentNullException(nameof(azureStorageService));
        }

        public override HandlerResult<IEnumerable<AssetDto>> HandleRequest(GetArchiveRequest request, CancellationToken cancellationToken, HandlerResult<IEnumerable<AssetDto>> result)
        {
            List<Asset> assets = new List<Asset>();
            var allVersions = _dbcontext.AssetVersions.Where(av => av.ActiveVersion == 1).ToList();
            var allUsers = _dbcontext.AppUsers.ToList();

            if (request.ExpiryDays >= 90)
            {
                assets = _dbcontext.Assets.Where(a => a.Status == Convert.ToInt32(AssetStatus.Archived) && a.StatusUpdatedDate < DateTimeOffset.UtcNow.AddDays(-90)).ToList();
            }
            else if (request.AssetId == 0)
            {
                assets = _dbcontext.Assets.Where(a => a.Status == Convert.ToInt32(AssetStatus.Archived)).ToList();
            } else
            {
                assets = _dbcontext.Assets.Where(a => a.Status == Convert.ToInt32(AssetStatus.Archived) && a.Id == request.AssetId).ToList();
            }

            result.Entity = _mapper.Map<IEnumerable<AssetDto>>(assets);

            // Update Asset URLs
            foreach (var asset in result.Entity)
            {
                var latestActiveVersion = allVersions.FirstOrDefault(av => av.AssetId == asset.Id);
                asset.Thumbnail = latestActiveVersion.Thumbnail;
                asset.OriginalUrl = latestActiveVersion.OriginalUrl;
                asset.Extension = latestActiveVersion.Extension;
                asset.FileType = latestActiveVersion.FileType;
                asset.CreatedByName = allUsers.First(u => u.Id == latestActiveVersion.CreatedById).UserName;
                asset.Size = Convert.ToInt32(latestActiveVersion.Size);
                asset.FileName = latestActiveVersion.FileName;
                asset.FileSizeText = latestActiveVersion.FileSizeText;
                asset.Key = latestActiveVersion.Key;
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