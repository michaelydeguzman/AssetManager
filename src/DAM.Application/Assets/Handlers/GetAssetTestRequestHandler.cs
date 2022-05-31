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
    public class GetAssetTestRequestHandler : HandlerBase<GetAssetTestRequest, HandlerResult<IEnumerable<AssetDto>>>
    {
        private readonly ICacheProvider _cacheProvider;
        private readonly IDbContext _dbcontext;
        private readonly IMapper _mapper;
        private readonly IConfiguration _configuration;

        public GetAssetTestRequestHandler(ICacheProvider cacheProvider, IDbContext dbcontext, IMapper mapper, IConfiguration configuration, IAzureStorageService azureStorageService)
        {
            _cacheProvider = cacheProvider ?? throw new ArgumentNullException(nameof(cacheProvider));
            _dbcontext = dbcontext ?? throw new ArgumentNullException(nameof(dbcontext));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
        }

        public override HandlerResult<IEnumerable<AssetDto>> HandleRequest(GetAssetTestRequest request, CancellationToken cancellationToken, HandlerResult<IEnumerable<AssetDto>> result)
        {
            if (request.Type == "a")
            { 
                var assets = _dbcontext.Assets.Where(a => a.Status != Convert.ToInt32(AssetStatus.Archived) && a.Status != Convert.ToInt32(AssetStatus.Deleted))
                    .Include(folder => folder.AssetAccounts).ThenInclude(aa => aa.Account)
                    .Include(folder => folder.AssetCountryRegions).ThenInclude(aa => aa.Region).ThenInclude(aa => aa.Country)
                    //.Include(asset => asset.AssetVersions).ThenInclude(av=>av.Tags)
                    .ToList();
                result.Entity = _mapper.Map<IEnumerable<AssetDto>>(assets);
                // Update Asset URLs
                foreach (var asset in result.Entity)
                {
                    #region Change Asset DTO with active asset version
                    AssetVersions version = new AssetVersions();
                    List<AssetVersions> versions = new List<AssetVersions>();
                    List<Tag> labels = new List<Tag>();
                    versions = _dbcontext.AssetVersions.Where(a => a.AssetId == asset.Id).ToList();
                    asset.AssetVersions = (List<AssetVersionsDto>)_mapper.Map<IEnumerable<AssetVersionsDto>>(versions);
                    //get active version
                    version = versions.Find((v) => v.ActiveVersion == 1);
                    //version = _dbcontext.AssetVersions.FirstOrDefault(a => a.AssetId == asset.Id && a.ActiveVersion == 1);
                    labels = _dbcontext.Tags.Where(a => a.AssetId == version.Id).ToList();
                    asset.Tags = (List<TagDto>)_mapper.Map<IEnumerable<TagDto>>(labels);
                    //for the public share url, directly download
                    asset.DownloadUrl = version.DownloadUrl; //_azureStorageService.GetBlobSasUri(_configuration, string.Concat(version.Key, ".", version.Extension), version.FileName, false);
                    asset.Thumbnail = version.Thumbnail; //_azureStorageService.GetBlobSasUri(_configuration, string.Concat(version.Key, ".", version.Extension), string.Empty, true);

                    //for the private share url, redirect to /asserts/assetsId
                    asset.CopyUrl = _configuration["BaseUrl"] + "Assets/" + version.Id;
                    asset.CreatedByName = _dbcontext.AppUsers.First(u => u.Id == version.CreatedById).UserName;
                    asset.StatusName = ((AssetStatus)version.Status).GetDescription();

                    //Enum.GetName(typeof(AssetStatus), asset.Status);
                    asset.Size = Convert.ToInt32(version.Size);
                    asset.Extension = version.Extension;
                    asset.FileName = version.FileName;
                    asset.FileSizeText = version.FileSizeText;
                    asset.FileType = version.FileType;
                    asset.Key = version.Key;
                    asset.OriginalUrl = version.OriginalUrl; //_azureStorageService.GetBlobSasUri(_configuration, string.Concat(version.Key, ".", version.Extension), string.Empty, false, true);
                    asset.DownloadCount = version.DownloadCount;
                    #endregion
                }
            }
            else if (request.Type == "c")
            {
                var assets = _dbcontext.Assets.Where(a => a.Status != Convert.ToInt32(AssetStatus.Archived) && a.Status != Convert.ToInt32(AssetStatus.Deleted)).ToList();
                var allVersions = _dbcontext.AssetVersions.ToList();
                var allTags = _dbcontext.Tags.ToList();
                var allUsers = _dbcontext.AppUsers.ToList();
                var allAccounts = _dbcontext.AssetAccounts.ToList();
                var allCountryRegions = _dbcontext.AssetCountryRegions.ToList();
                result.Entity = _mapper.Map<IEnumerable<AssetDto>>(assets);
                // Update Asset URLs
                foreach (var asset in result.Entity)
                {
                    #region Change Asset DTO with active asset version
                    AssetVersionsDto version = new AssetVersionsDto();
                    List<Tag> labels = new List<Tag>();
                    asset.AssetVersions = (List<AssetVersionsDto>)_mapper.Map<IEnumerable<AssetVersionsDto>>(allVersions.Where(a => a.AssetId == asset.Id));
                    version = asset.AssetVersions.Find((v) => v.ActiveVersion == 1);
                    labels = allTags.Where(a => a.AssetId == version.Id).ToList();
                    asset.Tags = (List<TagDto>)_mapper.Map<IEnumerable<TagDto>>(labels);
                    //for the public share url, directly download
                    asset.DownloadUrl = version.DownloadUrl; //_azureStorageService.GetBlobSasUri(_configuration, string.Concat(version.Key, ".", version.Extension), version.FileName, false);
                    asset.Thumbnail = version.Thumbnail; //_azureStorageService.GetBlobSasUri(_configuration, string.Concat(version.Key, ".", version.Extension), string.Empty, true);

                    //for the private share url, redirect to /asserts/assetsId
                    asset.CopyUrl = _configuration["BaseUrl"] + "Assets/" + version.Id;
                    asset.CreatedByName = allUsers.First(u => u.Id == version.CreatedById).UserName;
                    asset.StatusName = ((AssetStatus)version.Status).GetDescription();

                    //Enum.GetName(typeof(AssetStatus), asset.Status);
                    asset.Size = Convert.ToInt32(version.Size);
                    asset.Extension = version.Extension;
                    asset.FileName = version.FileName;
                    asset.FileSizeText = version.FileSizeText;
                    asset.FileType = version.FileType;
                    asset.Key = version.Key;
                    asset.OriginalUrl = version.OriginalUrl; //_azureStorageService.GetBlobSasUri(_configuration, string.Concat(version.Key, ".", version.Extension), string.Empty, false, true);
                    asset.DownloadCount = version.DownloadCount;
                    asset.Accounts = (List<AccountDto>)_mapper.Map<IEnumerable<AccountDto>>(allAccounts.Where(a => a.AssetId == asset.Id));
                    asset.Countries = (List<CountryDto>)_mapper.Map<IEnumerable<CountryDto>>(allCountryRegions.Where(a => a.AssetId == asset.Id));
                    asset.Regions = (List<RegionDto>)_mapper.Map<IEnumerable<RegionDto>>(allCountryRegions.Where(a => a.AssetId == asset.Id));
                    #endregion
                }
            }
            else if (request.Type == "d")
            {
                var assets = _dbcontext.Assets.Where(a => a.Status != Convert.ToInt32(AssetStatus.Archived) && a.Status != Convert.ToInt32(AssetStatus.Deleted)).ToList();
                var allVersions = _dbcontext.AssetVersions.ToList();
                var allTags = _cacheProvider.Get<List<Tag>>("allTags"); //_dbcontext.Tags.ToList(); 
                var allUsers = _dbcontext.AppUsers.ToList();
                var allAssetAccounts = _cacheProvider.Get<List<AccountDto>>("allAssetAccounts");//_dbcontext.AssetAccounts.ToList();
                var allCountries = _cacheProvider.Get<List<CountryDto>>("allCountries"); //_dbcontext.AssetCountryRegions.ToList();
                var allRegions = _cacheProvider.Get<List<RegionDto>>("allRegions");
                result.Entity = _mapper.Map<IEnumerable<AssetDto>>(assets);
                // Update Asset URLs
                foreach (var asset in result.Entity)
                {
                    #region Change Asset DTO with active asset version
                    AssetVersionsDto version = new AssetVersionsDto();
                    List<Tag> labels = new List<Tag>();
                    asset.AssetVersions = (List<AssetVersionsDto>)_mapper.Map<IEnumerable<AssetVersionsDto>>(allVersions.Where(a => a.AssetId == asset.Id));
                    version = asset.AssetVersions.Find((v) => v.ActiveVersion == 1);
                    labels = allTags.Where(a => a.AssetId == version.Id).ToList();
                    asset.Tags = (List<TagDto>)_mapper.Map<IEnumerable<TagDto>>(labels);
                    //for the public share url, directly download
                    asset.DownloadUrl = version.DownloadUrl; //_azureStorageService.GetBlobSasUri(_configuration, string.Concat(version.Key, ".", version.Extension), version.FileName, false);
                    asset.Thumbnail = version.Thumbnail; //_azureStorageService.GetBlobSasUri(_configuration, string.Concat(version.Key, ".", version.Extension), string.Empty, true);

                    //for the private share url, redirect to /asserts/assetsId
                    asset.CopyUrl = _configuration["BaseUrl"] + "Assets/" + version.Id;
                    asset.CreatedByName = allUsers.First(u => u.Id == version.CreatedById).UserName;
                    asset.StatusName = ((AssetStatus)version.Status).GetDescription();

                    //Enum.GetName(typeof(AssetStatus), asset.Status);
                    asset.Size = Convert.ToInt32(version.Size);
                    asset.Extension = version.Extension;
                    asset.FileName = version.FileName;
                    asset.FileSizeText = version.FileSizeText;
                    asset.FileType = version.FileType;
                    asset.Key = version.Key;
                    asset.OriginalUrl = version.OriginalUrl; //_azureStorageService.GetBlobSasUri(_configuration, string.Concat(version.Key, ".", version.Extension), string.Empty, false, true);
                    asset.DownloadCount = version.DownloadCount;
                    asset.Accounts = allAssetAccounts.Where(a => a.AssetId == asset.Id).ToList();
                    asset.Countries = allCountries.Where(a => a.AssetId == asset.Id).ToList();
                    asset.Regions = allRegions.Where(a => a.AssetId == asset.Id).ToList();
                    #endregion
                }
            }
            else if (request.Type == "e")
            {
                var assets = _dbcontext.Assets.Where(a => a.Status != Convert.ToInt32(AssetStatus.Archived) && a.Status != Convert.ToInt32(AssetStatus.Deleted)).ToList();
                var allVersions = _dbcontext.AssetVersions.ToList();
                var allTags = _dbcontext.Tags.ToList();
                var allUsers = _dbcontext.AppUsers.ToList();
                var allAssetAccounts = _dbcontext.AssetAccounts.ToList();
                var allAssetCountryRegions = _dbcontext.AssetCountryRegions.ToList();
                var allAccounts = _dbcontext.Accounts.ToList();
                var allCountries = _dbcontext.Countries.ToList();
                var allRegions = _dbcontext.Regions.ToList();
                result.Entity = _mapper.Map<IEnumerable<AssetDto>>(assets);
                // Update Asset URLs
                foreach (var asset in result.Entity)
                {
                    var accountIds = allAssetAccounts.Where(aa => aa.AssetId == asset.Id).Select(a => a.AccountId).ToList();
                    var countriesIds = allAssetCountryRegions.Where(acr => acr.AssetId == asset.Id).Select(a => a.CountryId).ToList();
                    var regionIds = allAssetCountryRegions.Where(acr => acr.AssetId == asset.Id).Select(a => a.RegionId).ToList();

                    #region Change Asset DTO with active asset version
                    AssetVersionsDto version = new AssetVersionsDto();
                    List<Tag> labels = new List<Tag>();
                    asset.AssetVersions = (List<AssetVersionsDto>)_mapper.Map<IEnumerable<AssetVersionsDto>>(allVersions.Where(a => a.AssetId == asset.Id));
                    //get active version
                    version = asset.AssetVersions.Find((v) => v.ActiveVersion == 1);
                    //version = _dbcontext.AssetVersions.FirstOrDefault(a => a.AssetId == asset.Id && a.ActiveVersion == 1);
                    labels = allTags.Where(a => a.AssetId == version.Id).ToList();
                    asset.Tags = (List<TagDto>)_mapper.Map<IEnumerable<TagDto>>(labels);
                    //for the public share url, directly download
                    asset.DownloadUrl = version.DownloadUrl; //_azureStorageService.GetBlobSasUri(_configuration, string.Concat(version.Key, ".", version.Extension), version.FileName, false);
                    asset.Thumbnail = version.Thumbnail; //_azureStorageService.GetBlobSasUri(_configuration, string.Concat(version.Key, ".", version.Extension), string.Empty, true);

                    //for the private share url, redirect to /asserts/assetsId
                    asset.CopyUrl = _configuration["BaseUrl"] + "Assets/" + version.Id;
                    asset.CreatedByName = allUsers.First(u => u.Id == version.CreatedById).UserName;
                    asset.StatusName = ((AssetStatus)version.Status).GetDescription();

                    //Enum.GetName(typeof(AssetStatus), asset.Status);
                    asset.Size = Convert.ToInt32(version.Size);
                    asset.Extension = version.Extension;
                    asset.FileName = version.FileName;
                    asset.FileSizeText = version.FileSizeText;
                    asset.FileType = version.FileType;
                    asset.Key = version.Key;
                    asset.OriginalUrl = version.OriginalUrl; //_azureStorageService.GetBlobSasUri(_configuration, string.Concat(version.Key, ".", version.Extension), string.Empty, false, true);
                    asset.DownloadCount = version.DownloadCount;
                    asset.Accounts = (List<AccountDto>)_mapper.Map<IEnumerable<AccountDto>>(allAccounts.Where(a => accountIds.Contains(a.Id)));
                    asset.Countries = (List<CountryDto>)_mapper.Map<IEnumerable<CountryDto>>(allCountries.Where(a => countriesIds.Contains(a.Id)));
                    asset.Regions = (List<RegionDto>)_mapper.Map<IEnumerable<RegionDto>>(allRegions.Where(a => regionIds.Contains(a.Id)));
                    #endregion
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