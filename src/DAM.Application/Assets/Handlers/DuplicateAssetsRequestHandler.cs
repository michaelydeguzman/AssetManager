using AutoMapper;
using Azure.Storage.Blobs.Models;
using DAM.Application.Assets.Dtos;
using DAM.Application.Assets.Enums;
using DAM.Application.Assets.Requests;
using DAM.Application.AuditTrail.Dtos;
using DAM.Application.AuditTrail.Enums;

using DAM.Application.Cache;
using DAM.Application.Extensions;
using DAM.Application.Services.Interfaces;
using DAM.Domain.Entities;
using DAM.Persistence;
using Force.Crc32;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading;

namespace DAM.Application.Assets.Handlers
{
    public class DuplicateAssetsRequestHandler : HandlerBase<DuplicateAssetsRequest, HandlerResult<IEnumerable<AssetDto>>>
    {
        private readonly ICacheProvider _cacheProvider;
        private readonly IDbContext _dbcontext;
        private readonly IMapper _mapper;
        private readonly IConfiguration _configuration;
        private readonly IAzureStorageService _azureStorageService;
        private readonly IConversionService _conversionService;
        private readonly ITagService _tagService;
        private readonly IHelperService _helperService;

        public DuplicateAssetsRequestHandler(ICacheProvider cacheProvider, IDbContext dbcontext, IMapper mapper, IConfiguration configuration, IAzureStorageService azureStorageService,
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

        public override HandlerResult<IEnumerable<AssetDto>> HandleRequest(DuplicateAssetsRequest request, CancellationToken cancellationToken, HandlerResult<IEnumerable<AssetDto>> result)
        {
            result.Entity = new List<AssetDto>();
           
            foreach (var assetId in request.AssetIds)
            {
                var assetToCopy = _dbcontext.Assets.FirstOrDefault(a => a.Id == assetId);

                // Copy asset into new asset row

                var newAsset = new Asset
                {
                    Title = assetToCopy.Title,
                    FolderId = request.FolderId,
                    Status = assetToCopy.Status,
                    StatusUpdatedDate = assetToCopy.StatusUpdatedDate,
                    Description = assetToCopy.Description,
                    ExpiryDate = assetToCopy.ExpiryDate,
                    CreatedById = request.UserId
                };

                _dbcontext.Assets.Add(newAsset);
                _dbcontext.SaveChanges();

                // Copy Asset versions 

                var assetVersionsToCopy = _dbcontext.AssetVersions.Where(a => a.AssetId == assetId);

                foreach (var version in assetVersionsToCopy)
                {
                    var newVersionKey = Guid.NewGuid().ToString().Replace("-", "");
                    var thumbnailExt = string.Empty;
                    bool copyThumbnail = true;

                    // copy blob to new key for asset, thumbnail, downloadUrl

                    _azureStorageService.CopyBlob(_configuration, version.Key + "." + version.Extension, newVersionKey + "." + version.Extension);
                    var newVersionUrl = _azureStorageService.GetBlobSasUri(_configuration, string.Concat(newVersionKey, ".", version.Extension), string.Empty, version.FileType, false);
                    var newDownloadUrl = _azureStorageService.GetBlobSasUri(_configuration, string.Concat(newVersionKey, ".", version.Extension), string.Empty, version.FileType, true);
                    var newThumbnailUrl = "";

                    if (version.FileType.Contains("audio"))
                    {
                        newThumbnailUrl = "audio";
                        copyThumbnail = false;
                    }
                    else if (version.FileType.Contains("otf") || version.FileType.Contains("ttf"))
                    {
                        newThumbnailUrl = "fonts";
                        copyThumbnail = false;
                    }
                    else if (version.FileType.Contains("eps"))
                    {
                        newThumbnailUrl = "eps";
                        copyThumbnail = false;
                    }
                    else if (version.FileType.Contains("zip"))
                    {
                        newThumbnailUrl = "zip";
                        copyThumbnail = false;
                    }
                    else if (version.FileType.Contains("svg"))
                    {
                        newThumbnailUrl = "svg";
                        copyThumbnail = false;
                    }
                    else if (version.FileType.Contains("image"))
                    {
                        if (version.Extension.Contains("tiff") || version.Extension.Contains("tif"))
                        {
                            thumbnailExt = "png";
                        } 
                        else
                        {
                            thumbnailExt = "Jpeg";
                        }
                    }
                    else
                    {
                        thumbnailExt = version.Extension;
                    }

                    if (copyThumbnail)
                    {
                        if (version.Thumbnail == version.Extension)
                        {
                            newThumbnailUrl = version.Extension;
                        }
                        else
                        {
                            _azureStorageService.CopyBlob(_configuration, version.Key + "." + thumbnailExt, newVersionKey + "." + thumbnailExt, true);
                            newThumbnailUrl = _azureStorageService.GetBlobSasUri(_configuration, string.Concat(newVersionKey, ".", thumbnailExt), string.Empty, version.FileType, true);
                        }
                    }

                    var newVersion = new AssetVersions
                    {
                        AssetId = newAsset.Id,
                        FileName = version.FileName,
                        Key = newVersionKey,
                        Extension = version.Extension,
                        Status = version.Status,
                        ActiveVersion = version.ActiveVersion,
                        StatusUpdatedDate = version.StatusUpdatedDate,
                        Description = version.Description,
                        Size = version.Size,
                        FileSizeText = version.FileSizeText,
                        FileType = version.FileType,
                        Version = version.Version,
                        DownloadCount = 0,
                        CRC32Code = version.CRC32Code,
                        OriginalUrl = newVersionUrl,
                        Thumbnail = newThumbnailUrl,
                        DownloadUrl = newDownloadUrl,
                        CreatedById = version.CreatedById
                    };

                    _dbcontext.AssetVersions.Add(newVersion);
                    _dbcontext.SaveChanges();

                    // Copy tags per version

                    var tagsToCopy = _dbcontext.Tags.Where(aa => aa.AssetId == version.Id);
                    var newTagsToCreate = new List<Tag>();

                    foreach (var tag in tagsToCopy)
                    {
                        var newTag = new Tag
                        {
                            AssetId = newVersion.Id,
                            IsCognitive = tag.IsCognitive,
                            Name = tag.Name

                        };
                        newTagsToCreate.Add(newTag);
                    }
                    _dbcontext.Tags.AddRange(newTagsToCreate);
                    _dbcontext.SaveChanges();
                }

                // Copy asset meta data

                // accounts
                var assetAccountsToCopy = _dbcontext.AssetAccounts.Where(aa => aa.AssetId == assetToCopy.Id);

                if (assetAccountsToCopy.Any())
                {
                    var accountsToCreate = new List<AssetAccountMetaData>();
                    foreach (var account in assetAccountsToCopy)
                    {
                        var newAssetAccount = new AssetAccountMetaData
                        {
                            AssetId = newAsset.Id,
                            AccountId = account.AccountId,
                            CreatedById = request.UserId
                        };
                        accountsToCreate.Add(newAssetAccount);
                    }
                    _dbcontext.AssetAccounts.AddRange(accountsToCreate);
                    _dbcontext.SaveChanges();
                }

                // countries

                var assetRegionsToCopy = _dbcontext.AssetCountryRegions.Where(aa => aa.AssetId == assetToCopy.Id);

                if (assetRegionsToCopy.Any())
                {
                    var assetRegionsToCreate = new List<AssetCountryRegionMetaData>();
                    foreach (var assetRegion in assetRegionsToCopy)
                    {
                        var newAssetRegion = new AssetCountryRegionMetaData
                        {
                            AssetId = newAsset.Id,
                            RegionId = assetRegion.RegionId,
                            CountryId = assetRegion.CountryId,
                            CreatedById = request.UserId
                        };
                        assetRegionsToCreate.Add(newAssetRegion);
                    }
                    _dbcontext.AssetCountryRegions.AddRange(assetRegionsToCreate);
                    _dbcontext.SaveChanges();
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