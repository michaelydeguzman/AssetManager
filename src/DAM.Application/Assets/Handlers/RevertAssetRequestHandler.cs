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
    public class RevertAssetRequestHandler : HandlerBase<RevertAssetRequest, HandlerResult<IEnumerable<AssetDto>>>
    {
        private readonly ICacheProvider _cacheProvider;
        private readonly IDbContext _dbcontext;
        private readonly IMapper _mapper;
        private readonly IConfiguration _configuration;
        private readonly IAzureStorageService _azureStorageService;
        private readonly IConversionService _conversionService;
        private readonly ITagService _tagService;
        private readonly IHelperService _helperService;

        public RevertAssetRequestHandler(ICacheProvider cacheProvider, IDbContext dbcontext, IMapper mapper, IConfiguration configuration, IAzureStorageService azureStorageService,
          IConversionService conversionService, ITagService tagService, IHelperService helperService)
        {
            _cacheProvider = cacheProvider ?? throw new ArgumentNullException(nameof(cacheProvider));
            _dbcontext = dbcontext ?? throw new ArgumentNullException(nameof(dbcontext));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
            _azureStorageService = azureStorageService ?? throw new ArgumentNullException(nameof(azureStorageService));
            _conversionService = conversionService ?? throw new ArgumentNullException(nameof(conversionService));
            _tagService = tagService ?? throw new ArgumentNullException(nameof(tagService));
            _helperService = helperService ?? throw new ArgumentNullException(nameof(helperService));
        }

        public override HandlerResult<IEnumerable<AssetDto>> HandleRequest(RevertAssetRequest request, CancellationToken cancellationToken, HandlerResult<IEnumerable<AssetDto>> result)
        {
            List<Asset> assets = new List<Asset>();

            assets = _dbcontext.Assets.Where(a => a.Status != Convert.ToInt32(AssetStatus.Archived) && a.Status != Convert.ToInt32(AssetStatus.Deleted) && a.Id == request.AssetId)
                //.Include(folder => folder.AssetAccounts).ThenInclude(aa => aa.Account)
                //.Include(folder => folder.AssetCountryRegions).ThenInclude(aa => aa.Region).ThenInclude(aa => aa.Country)
                //.Include(asset => asset.AssetVersions).ThenInclude(av => av.Tags)
                .ToList();

            result.Entity = _mapper.Map<IEnumerable<AssetDto>>(assets);

            // Update Asset URLs
            foreach (var asset in result.Entity)
            {
                #region Change Asset DTO with active asset version

                AssetVersions activeVersion = new AssetVersions();
                AssetVersions newActiveVersion = new AssetVersions();
                List<AssetVersions> versions = new List<AssetVersions>();

                versions = _dbcontext.AssetVersions.Where(a => a.AssetId == asset.Id).ToList();
                asset.AssetVersions = (List<AssetVersionsDto>)_mapper.Map<IEnumerable<AssetVersionsDto>>(versions);
                //get active version
                activeVersion = _dbcontext.AssetVersions.FirstOrDefault(a => a.AssetId == asset.Id && a.ActiveVersion == 1);
                newActiveVersion = _dbcontext.AssetVersions.FirstOrDefault(a => a.AssetId == asset.Id && a.Id == request.VersionAssetId);
                if (activeVersion != null)
                {
                    if (newActiveVersion != null)
                    {
                        activeVersion.ActiveVersion = 0;
                        activeVersion.ModifiedDate = DateTime.Now;

                        newActiveVersion.ActiveVersion = 1;
                        newActiveVersion.ModifiedDate = DateTime.Now;
                        _dbcontext.AssetVersions.Update(activeVersion);
                        _dbcontext.AssetVersions.Update(newActiveVersion);
                        _dbcontext.SaveChanges();
                    }

                }

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