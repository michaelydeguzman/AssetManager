using AutoMapper;
using Azure.Storage.Blobs;
using DAM.Application.Accounts.Dtos;
using DAM.Application.Assets.Dtos;
using DAM.Application.Assets.Requests;
using DAM.Application.Cache;
using DAM.Application.CountryRegions.Dtos;
using DAM.Application.Users.Constants;
using DAM.Application.Users.Dtos;
using DAM.Application.Users.Requests;
using DAM.Domain.Entities;
using DAM.Persistence;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Configuration;
using Azure.Storage;
using Microsoft.Extensions.Configuration;
using Microsoft.EntityFrameworkCore;
using DAM.Application.Assets.Enums;
using System.IO;
using System.Net;
using DAM.Application.Services.Interfaces;
using System.Reflection;
using System.ComponentModel;
using DAM.Application.Extensions;

namespace DAM.Application.Assets.Handlers
{
    public class GetAssetsByStatusRequestHandler : HandlerBase<GetAssetsByStatusRequest, HandlerResult<IEnumerable<AssetDto>>>
    {
        private readonly ICacheProvider _cacheProvider;
        private readonly IDbContext _dbcontext;
        private readonly IMapper _mapper;
        private readonly IConfiguration _configuration;
        private readonly IAzureStorageService _azureStorageService;

        public GetAssetsByStatusRequestHandler(ICacheProvider cacheProvider, IDbContext dbcontext, IMapper mapper, IConfiguration configuration, IAzureStorageService azureStorageService)
        {
            _cacheProvider = cacheProvider ?? throw new ArgumentNullException(nameof(cacheProvider));
            _dbcontext = dbcontext ?? throw new ArgumentNullException(nameof(dbcontext));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
            _azureStorageService = azureStorageService ?? throw new ArgumentNullException(nameof(azureStorageService));
        }
        
        public override HandlerResult<IEnumerable<AssetDto>> HandleRequest(GetAssetsByStatusRequest request, CancellationToken cancellationToken, HandlerResult<IEnumerable<AssetDto>> result)
        {
            List<Asset> assets = new List<Asset>();
            List<AccountDto> accounts = new List<AccountDto>();
            List<RegionDto> regions = new List<RegionDto>();
            List<TagDto> tags = new List<TagDto>();

            assets = _dbcontext.Assets.Where(a => a.Status != Convert.ToInt32(AssetStatus.Archived) && a.Status == request.AssetStatus)
                .Include(folder => folder.AssetAccounts).ThenInclude(aa => aa.Account)
                .Include(folder => folder.AssetCountryRegions).ThenInclude(aa => aa.Region).ThenInclude(aa => aa.Country)
                .Include(asset => asset.AssetVersions).ThenInclude(av => av.Tags)
                .ToList();

            result.Entity = _mapper.Map<IEnumerable<AssetDto>>(assets);

            // Update Asset URLs
            foreach (var asset in result.Entity)
            {
                asset.DownloadUrl = _azureStorageService.GetBlobSasUri(_configuration, string.Concat(asset.Key, ".", asset.Extension), asset.FileName, string.Empty, false, true);
                asset.Thumbnail = _azureStorageService.GetBlobSasUri(_configuration, string.Concat(asset.Key, ".", asset.Extension), string.Empty, string.Empty, true);
                asset.CopyUrl = _configuration["BaseUrl"] + "Assets/Download/" + asset.Key + "/" + asset.Id;
                asset.CreatedByName = _dbcontext.AppUsers.First(u => u.Id == asset.CreatedById).UserName;
                asset.StatusName = ((AssetStatus)asset.Status).GetDescription();
                Enum.GetName(typeof(AssetStatus), asset.Status);
                if (asset.Extension == "tiff" || asset.Extension == "tif")
                {
                    asset.OriginalUrl = _azureStorageService.GetBlobSasUri(_configuration, string.Concat(asset.Key, ".", "png"), string.Empty, asset.FileType, false);
                }
                else
                {
                    asset.OriginalUrl = _azureStorageService.GetBlobSasUri(_configuration, string.Concat(asset.Key, ".", asset.Extension), string.Empty, asset.FileType, false);
                }
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