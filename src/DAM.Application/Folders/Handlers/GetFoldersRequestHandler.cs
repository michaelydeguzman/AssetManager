using AutoMapper;
using DAM.Application.Accounts.Dtos;
using DAM.Application.Assets.Enums;
using DAM.Application.Assets.Helpers;
using DAM.Application.Cache;
using DAM.Application.Companies.Dtos;
using DAM.Application.CountryRegions.Dtos;
using DAM.Application.Folders.Dtos;
using DAM.Application.Folders.Requests;
using DAM.Application.Services.Interfaces;
using DAM.Domain.Entities;
using DAM.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using Z.EntityFramework.Plus;
using static DAM.Application.Folders.Requests.FoldersRequest;

namespace DAM.Application.Folders.Handlers
{
    public class GetFoldersRequestHandler : HandlerBase<GetFolderRequest, HandlerResult<IEnumerable<FolderDto>>>
    {
        private readonly ICacheProvider _cacheProvider;
        private readonly IDbContext _dbcontext;
        private readonly IMapper _mapper;
        private readonly IConfiguration _configuration;
        private readonly IAzureStorageService _azureStorageService;

        public GetFoldersRequestHandler(ICacheProvider cacheProvider, IDbContext dbcontext, IMapper mapper, IConfiguration configuration, IAzureStorageService azureStorageService)
        {
            _cacheProvider = cacheProvider ?? throw new ArgumentNullException(nameof(cacheProvider));
            _dbcontext = dbcontext ?? throw new ArgumentNullException(nameof(dbcontext));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
            _azureStorageService = azureStorageService ?? throw new ArgumentNullException(nameof(azureStorageService));
        }

        public override HandlerResult<IEnumerable<FolderDto>> HandleRequest(GetFolderRequest request, CancellationToken cancellationToken, HandlerResult<IEnumerable<FolderDto>> result)
        {
            List<Folder> folders = new List<Folder>();
            //var allAssets = _dbcontext.Assets.Where(a => a.Status != Convert.ToInt32(AssetStatus.Archived) && a.Status != Convert.ToInt32(AssetStatus.Deleted));
            //var allAccounts = _dbcontext.Accounts.ToList();
            //var allCountryRegions = _dbcontext.Countries.ToList();
            //var allFolderAccounts = _dbcontext.FolderAccounts.ToList();
            //var allFolderCountryRegions = _dbcontext.FolderCountryRegions.ToList();
            var allCompany = _dbcontext.Companies.ToList();

            if (request.FolderId == 0)
            {
                folders = _dbcontext.Folders.Where(a => !a.Deleted)
                    //.Include(folder => folder.FolderAccounts)//.ThenInclude(fa => fa.Account)
                    //.Include(folder => folder.FolderCountryRegions).ThenInclude(fr => fr.Country)//.ThenInclude(fr => fr.Region)
                    //.Include(folder => folder.Company)
                    //.IncludeFilter(folder => folder.Assets.Where(a => a.Status != Convert.ToInt32(AssetStatus.Archived) && a.Status != Convert.ToInt32(AssetStatus.Deleted)))
                    .ToList();
            }
            else
            {
                folders = _dbcontext.Folders.Where(a => !a.Deleted && a.Id == request.FolderId)
                    //.IncludeFilter(folder => folder.Assets.Where(a => a.Status == Convert.ToInt32(AssetStatus.Draft)))
                    .Include(folder => folder.FolderAccounts)
                    .Include(folder => folder.FolderCountryRegions).ThenInclude(fr => fr.Region).ThenInclude(fr => fr.Country)
                    .ToList();
            }

            result.Entity = _mapper.Map<IEnumerable<FolderDto>>(folders);

            //if (request.FolderId > 0)
            //{
            //    foreach (var folder in result.Entity)
            //    {
            //        foreach (var asset in folder.Assets)
            //        {
            //            asset.DownloadUrl = _azureStorageService.GetBlobSasUri(_configuration, string.Concat(asset.Key, ".", asset.Extension), asset.FileName, string.Empty, false, true);
            //            asset.Thumbnail = _azureStorageService.GetBlobSasUri(_configuration, string.Concat(asset.Key, ".", asset.Extension), string.Empty, string.Empty, true);
            //            asset.OriginalUrl = _azureStorageService.GetBlobSasUri(_configuration, string.Concat(asset.Key, ".", asset.Extension), string.Empty, asset.FileType, false);
            //        }
            //    }
            //}

            if (result.Entity.Any())
            {
                foreach (var folder in result.Entity)
                {
                    //var accountIds = allFolderAccounts.Where(aa => aa.FolderId == folder.Id).Select(a => a.AccountId).ToList();
                    //var countriesIds = allFolderCountryRegions.Where(acr => acr.FolderId == folder.Id).Select(a => a.CountryId).ToList();
                    var company = _mapper.Map<List<CompanyDto>>(allCompany.Where(c => c.RootFolderId == folder.Id));

                    folder.Company = company;
                    //if (accountIds.Count > 0)
                    //{
                    //    var accounts = _mapper.Map<List<AccountDto>>(allAccounts.Where(a => accountIds.Contains(a.Id)));
                    //    folder.Accounts = accounts;
                    //}
                    //if (countriesIds.Count > 0)
                    //{
                    //    var countries = _mapper.Map<List<CountryDto>>(allCountryRegions.Where(a => countriesIds.Contains(a.Id)));
                    //    folder.Countries = countries;
                    //}
                    //folder.AssetCount = allAssets.Count(a => a.FolderId == folder.Id);
                }
            }

            //Deal with count of share assets
            //var shareFolders = "";
            //foreach(var a in allAssets)
            //{
            //    if(!String.IsNullOrEmpty(a.ShareFolderIds)) 
            //    {
            //        shareFolders = string.Concat(shareFolders, ',', a.ShareFolderIds);
            //    }
            //}    

            //if(!String.IsNullOrEmpty(shareFolders))
            //{
            //    var folderList = shareFolders.Split(',', StringSplitOptions.RemoveEmptyEntries);
            //    foreach(var fId in folderList)
            //    {
            //        var index = result.Entity.ToList().FindIndex((f) => f.Id.ToString() == fId);
            //        if(index > 0) 
            //        {
            //            result.Entity.ToList()[index].AssetCount += 1;
            //        }
            //    }
            //}

            result.ResultType = ResultType.Success;
            return result;
        }

        public override Exception OnException(Exception ex)
        {
            return new Exception(ex.Message, ex);
        }
    }
}
