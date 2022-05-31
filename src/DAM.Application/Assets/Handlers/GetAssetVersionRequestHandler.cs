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
    public class GetAssetVersionRequestHandler : HandlerBase<GetAssetVersionRequest, HandlerResult<IEnumerable<AssetDto>>>
    {
        private readonly ICacheProvider _cacheProvider;
        private readonly IDbContext _dbcontext;
        private readonly IMapper _mapper;
        private readonly IConfiguration _configuration;
        private readonly IAzureStorageService _azureStorageService;

        public GetAssetVersionRequestHandler(ICacheProvider cacheProvider, IDbContext dbcontext, IMapper mapper, IConfiguration configuration, IAzureStorageService azureStorageService)
        {
            _cacheProvider = cacheProvider ?? throw new ArgumentNullException(nameof(cacheProvider));
            _dbcontext = dbcontext ?? throw new ArgumentNullException(nameof(dbcontext));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
            _azureStorageService = azureStorageService ?? throw new ArgumentNullException(nameof(azureStorageService));
        }

        public override HandlerResult<IEnumerable<AssetDto>> HandleRequest(GetAssetVersionRequest request, CancellationToken cancellationToken, HandlerResult<IEnumerable<AssetDto>> result)
        {
            var allVersions = _dbcontext.AssetVersions.ToList();
            var allTags = _dbcontext.Tags.ToList();
            var allUsers = _dbcontext.AppUsers.ToList();
            var allAccounts = _dbcontext.AssetAccounts.ToList();
            var allCountryRegions = _dbcontext.AssetCountryRegions.ToList();

            var assets = _dbcontext.Assets.Where(a => a.Status != Convert.ToInt32(AssetStatus.Archived) && a.Status != Convert.ToInt32(AssetStatus.Deleted) && a.Id == request.AssetId)
                //.Include(folder => folder.AssetAccounts).ThenInclude(aa => aa.Account)
                //.Include(folder => folder.AssetCountryRegions).ThenInclude(aa => aa.Region).ThenInclude(aa => aa.Country)
                //.Include(asset => asset.AssetVersions).ThenInclude(av => av.Tags)
                .ToList();

            result.Entity = _mapper.Map<IEnumerable<AssetDto>>(assets);

            // Update Asset URLs
            foreach (var asset in result.Entity)
            {
                #region Change Asset DTO with active asset version
                AssetVersionsDto version = new AssetVersionsDto();
                List<Tag> labels = new List<Tag>();
                asset.AssetVersions = (List<AssetVersionsDto>)_mapper.Map<IEnumerable<AssetVersionsDto>>(allVersions.Where(a => a.AssetId == asset.Id));
                //get active version
                version = asset.AssetVersions.Find(a => a.Id == request.AssetVersionId);
                labels = allTags.Where(a => a.AssetId == version.Id).ToList();
                asset.Tags = (List<TagDto>)_mapper.Map<IEnumerable<TagDto>>(labels);
                //for the public share url, directly download
                asset.DownloadUrl = version.DownloadUrl; //_azureStorageService.GetBlobSasUri(_configuration, string.Concat(version.Key, ".", version.Extension), version.FileName, false);
                asset.Thumbnail = version.Thumbnail; //_azureStorageService.GetBlobSasUri(_configuration, string.Concat(version.Key, ".", version.Extension), string.Empty, true);
                //for the private share url, redirect to /asserts/assetsId
                asset.CopyUrl = _configuration["BaseUrl"] + "Assets/" + asset.Id;
                asset.CreatedByName = _dbcontext.AppUsers.First(u => u.Id == version.CreatedById).UserName;
                asset.StatusName = ((AssetStatus)version.Status).GetDescription();
                //Enum.GetName(typeof(AssetStatus), asset.Status);
                asset.Size = Convert.ToInt32(version.Size);
                asset.Extension = version.Extension;
                asset.FileName = version.FileName;
                asset.FileSizeText = version.FileSizeText;
                asset.FileType = version.FileType;
                asset.OriginalUrl = version.OriginalUrl;
                asset.DownloadCount = version.DownloadCount;
                asset.Accounts = (List<AccountDto>)_mapper.Map<IEnumerable<AccountDto>>(allAccounts.Where(a => a.AssetId == asset.Id));
                asset.Countries = (List<CountryDto>)_mapper.Map<IEnumerable<CountryDto>>(allCountryRegions.Where(a => a.AssetId == asset.Id));
                asset.Regions = (List<RegionDto>)_mapper.Map<IEnumerable<RegionDto>>(allCountryRegions.Where(a => a.AssetId == asset.Id));
                #endregion
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