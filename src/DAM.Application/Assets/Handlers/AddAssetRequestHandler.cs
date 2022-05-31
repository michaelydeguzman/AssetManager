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
    public class AddAssetRequestHandler : HandlerBase<AddAssetRequest, HandlerResult<AssetDto>>
    {
        private readonly ICacheProvider _cacheProvider;
        private readonly IDbContext _dbcontext;
        private readonly IMapper _mapper;
        private readonly IConfiguration _configuration;
        private readonly IAzureStorageService _azureStorageService;
        private readonly IConversionService _conversionService;
        private readonly ITagService _tagService;
        private readonly IHelperService _helperService;

        public AddAssetRequestHandler(ICacheProvider cacheProvider, IDbContext dbcontext, IMapper mapper, IConfiguration configuration, IAzureStorageService azureStorageService, 
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

        public override HandlerResult<AssetDto> HandleRequest(AddAssetRequest request, CancellationToken cancellationToken, HandlerResult<AssetDto> result)
        {
            var assetDto = request.AssetDto;
            var asset = _mapper.Map<Asset>(request.AssetDto);
            var createdBy = request.UserId;

            // if createdby is null, default user
            if (string.IsNullOrEmpty(request.UserId))
            {
                asset.CreatedById = _dbcontext.AppUsers.First(u => u.Email == _configuration["DefaultUser"]).Id;
            } 
            else
            {
                asset.CreatedById = createdBy;
            }

            if (request.AssetDto.FolderId == 0)
            {   
                asset.FolderId = null;
            }
            var tagList = new List<string>();
            asset.Status = Convert.ToInt32(AssetStatus.Draft);
            AssetVersions version = new AssetVersions();
            version.Key = Guid.NewGuid().ToString().Replace("-", "");
            BlobProperties ap = new BlobProperties();
            var thumbnail = "";
            var original = "";
            Stream thumbnailStream;

            using (Stream originalStream = new MemoryStream(request.AssetDto.FileBytes))
            {
                thumbnailStream = originalStream;
                bool convertThumbnailsuccess = false;

                if (request.AssetDto.FileType.Contains("video"))
                {
                    try
                    {
                        thumbnailStream = _conversionService.GetVideoThumbnail(_configuration, originalStream, string.Concat(version.Key, ".", assetDto.Extension), version.Key);
                        convertThumbnailsuccess = true;
                    }
                    catch (Exception ex)
                    {
                        convertThumbnailsuccess = false;
                    }
                }
                else if (request.AssetDto.Extension.Contains("doc"))
                {
                    try
                    {
                        thumbnailStream = _conversionService.GetDocumentThumbnail(originalStream);
                        convertThumbnailsuccess = true;
                    }
                    catch (Exception ex)
                    {
                        convertThumbnailsuccess = false;
                    }
                }
                else if (request.AssetDto.Extension.Contains("pdf"))
                {
                    try
                    {
                        thumbnailStream = _conversionService.GetPDFThumbnail(_configuration, originalStream);
                        convertThumbnailsuccess = true;
                    }
                    catch (Exception ex)
                    {
                        convertThumbnailsuccess = false;
                    }
                }
                else if (request.AssetDto.Extension.Contains("xls"))
                {
                    try
                    {
                        thumbnailStream = _conversionService.GetXLSThumbnail(originalStream);
                        convertThumbnailsuccess = true;
                    }
                    catch (Exception ex)
                    {
                        convertThumbnailsuccess = false;
                    }
                }
                else if (request.AssetDto.Extension.Contains("ppt"))
                {
                    try
                    {
                        thumbnailStream = _conversionService.GetPPTThumbnail(originalStream);
                        convertThumbnailsuccess = true;
                    }
                    catch (Exception ex)
                    {
                        convertThumbnailsuccess = false;
                    }
                }

                if(convertThumbnailsuccess)
                {
                    thumbnailStream = _conversionService.GetImageThumbnail(_configuration, thumbnailStream, assetDto.Extension);
                    BlobProperties thumbnailProps = _azureStorageService.UploadFileToStorage(_configuration, thumbnailStream, string.Concat(version.Key, ".", assetDto.Extension), out Uri thumbnailUri, true);
                    thumbnail = _azureStorageService.GetBlobSasUri(_configuration, string.Concat(version.Key, ".", assetDto.Extension), string.Empty,string.Empty, true);
                    ap = _azureStorageService.UploadFileToStorage(_configuration, originalStream, string.Concat(version.Key, ".", assetDto.Extension), out Uri assetUri);
                    original = _azureStorageService.GetBlobSasUri(_configuration, string.Concat(version.Key, ".", assetDto.Extension), string.Empty, assetDto.FileType, false);
                }
                else if (request.AssetDto.FileType.Contains("audio"))
                {
                    thumbnail = "audio";
                    ap = _azureStorageService.UploadFileToStorage(_configuration, originalStream, string.Concat(version.Key, ".", assetDto.Extension), out Uri assetUri);
                    original = _azureStorageService.GetBlobSasUri(_configuration, string.Concat(version.Key, ".", assetDto.Extension), string.Empty, assetDto.FileType, false);
                }
                else if (request.AssetDto.Extension.Contains("otf") || request.AssetDto.Extension.Contains("ttf"))
                {
                    thumbnail = "fonts";
                    ap = _azureStorageService.UploadFileToStorage(_configuration, originalStream, string.Concat(version.Key, ".", assetDto.Extension), out Uri assetUri);
                    original = _azureStorageService.GetBlobSasUri(_configuration, string.Concat(version.Key, ".", assetDto.Extension), string.Empty, assetDto.FileType, false);
                }
                else if (request.AssetDto.Extension.Contains("eps"))
                {
                    thumbnail = "eps";
                    ap = _azureStorageService.UploadFileToStorage(_configuration, originalStream, string.Concat(version.Key, ".", assetDto.Extension), out Uri assetUri);
                    original = _azureStorageService.GetBlobSasUri(_configuration, string.Concat(version.Key, ".", assetDto.Extension), string.Empty, assetDto.FileType, false);
                }
                else if (request.AssetDto.Extension.Contains("zip"))
                {
                    thumbnail = "zip";
                    ap = _azureStorageService.UploadFileToStorage(_configuration, originalStream, string.Concat(version.Key, ".", assetDto.Extension), out Uri assetUri);
                    original = _azureStorageService.GetBlobSasUri(_configuration, string.Concat(version.Key, ".", assetDto.Extension), string.Empty, assetDto.FileType, false);
                }
                else if (request.AssetDto.Extension.Contains("svg"))
                {
                    thumbnail = "svg";
                    ap = _azureStorageService.UploadFileToStorage(_configuration, originalStream, string.Concat(version.Key, ".", assetDto.Extension), out Uri assetUri);
                    original = _azureStorageService.GetBlobSasUri(_configuration, string.Concat(version.Key, ".", assetDto.Extension), string.Empty, assetDto.FileType, false);
                }
                else if(request.AssetDto.FileType.Contains("image"))
                {
                    // TIFF
                    if (request.AssetDto.Extension.Contains("tiff") || request.AssetDto.Extension.Contains("tif"))
                    {
                        thumbnailStream = _conversionService.GetImageThumbnail(_configuration, originalStream, assetDto.Extension);
                        BlobProperties thumbnailProps = _azureStorageService.UploadFileToStorage(_configuration, thumbnailStream, string.Concat(version.Key, ".", "png"), out Uri thumbnailUri, true);
                        thumbnail = _azureStorageService.GetBlobSasUri(_configuration, string.Concat(version.Key, ".", "png"), string.Empty, string.Empty, true); //thumbnailUri.AbsoluteUri;
                        ap = _azureStorageService.UploadFileToStorage(_configuration, originalStream, string.Concat(version.Key, ".", assetDto.Extension), out Uri assetUri);
                        Stream preview = _conversionService.GetPNGFromTIFF(originalStream);
                        BlobProperties previewProps = _azureStorageService.UploadFileToStorage(_configuration, preview, string.Concat(version.Key, ".", "png"), out Uri previewUri, true);
                        original = _azureStorageService.GetBlobSasUri(_configuration, string.Concat(version.Key, ".", "png"), string.Empty, assetDto.FileType, true);
                    }
                    else if(request.AssetDto.FileType.Contains("webp"))
                    {
                        thumbnailStream = _conversionService.GetMagickImageThumbnail(_configuration,originalStream, assetDto.Extension); 
                        //BlobProperties thumbnailProps = _azureStorageService.UploadFileToStorage(_configuration, originalStream, string.Concat(version.Key, ".", assetDto.Extension), out Uri thumbnailUri, true);
                        //thumbnail = _azureStorageService.GetBlobSasUri(_configuration, string.Concat(version.Key, ".", assetDto.Extension), string.Empty, string.Empty, true); //thumbnailUri.AbsoluteUri;
                        ap = _azureStorageService.UploadFileToStorage(_configuration, originalStream, string.Concat(version.Key, ".", assetDto.Extension), out Uri assetUri);
                        original = _azureStorageService.GetBlobSasUri(_configuration, string.Concat(version.Key, ".", assetDto.Extension), string.Empty, assetDto.FileType, false);
                        thumbnail = original;
                    }
                    else if (request.AssetDto.FileType.ToLower().Contains("nef") || request.AssetDto.FileType.ToLower().Contains("raw") || request.AssetDto.FileType.ToLower().Contains("heic") || request.AssetDto.FileType.ToLower().Contains("dng"))
                    {
                        thumbnail = request.AssetDto.Extension;
                        ap = _azureStorageService.UploadFileToStorage(_configuration, originalStream, string.Concat(version.Key, ".", assetDto.Extension), out Uri assetUri);
                        original = _azureStorageService.GetBlobSasUri(_configuration, string.Concat(version.Key, ".", assetDto.Extension), string.Empty, assetDto.FileType, false);
                    }
                    else
                    {
                        thumbnailStream = _conversionService.GetImageThumbnail(_configuration, originalStream, assetDto.Extension);
                        BlobProperties thumbnailProps = _azureStorageService.UploadFileToStorage(_configuration, thumbnailStream, string.Concat(version.Key, ".", "Jpeg"), out Uri thumbnailUri, true);
                        thumbnail = _azureStorageService.GetBlobSasUri(_configuration, string.Concat(version.Key, ".", "Jpeg"), string.Empty, string.Empty, true); //thumbnailUri.AbsoluteUri;
                        ap = _azureStorageService.UploadFileToStorage(_configuration, originalStream, string.Concat(version.Key, ".", assetDto.Extension), out Uri assetUri);
                        original = _azureStorageService.GetBlobSasUri(_configuration, string.Concat(version.Key, ".", assetDto.Extension), string.Empty, assetDto.FileType, false);

                    }
                }
                else
                {
                    thumbnail = request.AssetDto.Extension;
                    ap = _azureStorageService.UploadFileToStorage(_configuration, originalStream, string.Concat(version.Key, ".", assetDto.Extension), out Uri assetUri);
                    original = _azureStorageService.GetBlobSasUri(_configuration, string.Concat(version.Key, ".", assetDto.Extension), string.Empty, assetDto.FileType, false);
                }
                byte[] bytes = new byte[originalStream.Length];
                uint crc = Crc32Algorithm.Compute(bytes);
                version.CRC32Code = crc;

                
            }

            _dbcontext.Assets.Add(asset);
            _dbcontext.SaveChanges();

            //save asset versions

            version.AssetId = asset.Id;
            version.ActiveVersion = 1;
            version.Description = asset.Description;
            version.Extension = assetDto.Extension;
            version.FileName = assetDto.FileName;
            version.FileType = assetDto.FileType;
            version.Status = asset.Status;
            version.StatusUpdatedDate = asset.StatusUpdatedDate;
            version.CreatedById = asset.CreatedById;
            version.ModifiedById = asset.ModifiedById;
            version.ModifiedDate = asset.ModifiedDate;
            version.Version = 1;
            version.DownloadUrl = _azureStorageService.GetBlobSasUri(_configuration, string.Concat(version.Key, ".", assetDto.Extension), version.FileName, string.Empty, false, true);
            version.Size = ap.ContentLength;
            version.FileSizeText = _helperService.GetFileSize(ap.ContentLength);
            version.Thumbnail = thumbnail;
            version.OriginalUrl = original;
            _dbcontext.AssetVersions.Add(version);
            _dbcontext.SaveChanges();

            // Add tags
            if (request.AssetDto.Extension != "svg"
                && request.AssetDto.Extension.ToLower() != "nef"
                && request.AssetDto.Extension.ToLower() != "raw"
                && request.AssetDto.Extension.ToLower() != "dng"
                && request.AssetDto.Extension.ToLower() != "heic"
                && (request.AssetDto.FileType.Contains("image")
                || request.AssetDto.FileType.Contains("pdf") 
                || request.AssetDto.FileType.Contains("doc") 
                || request.AssetDto.FileType.Contains("ppt")))
            {
                var tags = _tagService.GetImageTags(_configuration, thumbnailStream).GetAwaiter().GetResult();
                foreach (var tag in tags)
                {
                    TextInfo textInfo = new CultureInfo("en-US", false).TextInfo;

                    _dbcontext.Tags.Add(new Tag()
                    {
                        AssetId = version.Id,
                        Name = textInfo.ToTitleCase(tag.ToLower()),
                        IsCognitive = true
                    });
                    tagList.Add(textInfo.ToTitleCase(tag.ToLower()));
                }
            }

            var accounts = request.AssetDto.Accounts;

            bool hasFolder = asset.FolderId != null;
            var folderName = hasFolder ? _dbcontext.Folders.First(f => f.Id == asset.FolderId).FolderName : null;

            var accountList = new List<string>();
            // Add account metadata
            if (accounts != null)
            {
                // asset has own specified accounts 
                foreach (var account in accounts)
                {
                    _dbcontext.AssetAccounts.Add(new AssetAccountMetaData()
                    {
                        AssetId = (int)(asset.Id),
                        AccountId = (int)(account.Id)
                    });
                }
            } else
            {
                if (hasFolder)
                {
                    // check if folder has account metadata mapping, if yes, apply to asset as well
                    var folderAccounts = _dbcontext.FolderAccounts.Include(fa=>fa.Account).Where(f => f.FolderId == asset.FolderId).ToList();

                    foreach(var folderAccount in folderAccounts)
                    {
                        _dbcontext.AssetAccounts.Add(new AssetAccountMetaData()
                        {
                            AssetId = (int)(asset.Id),
                            AccountId = (folderAccount.AccountId)
                        });
                        accountList.Add(folderAccount.Account.Description);
                    }
                }
            }

            // Add country/region metadata
            var regions = request.AssetDto.Regions;
            var regionList = new List<string>();
            var countryList = new List<string>();

            if (regions != null)
            {
                // asset has own specified country/region 
                foreach (var region in regions)
                {
                    _dbcontext.AssetCountryRegions.Add(new AssetCountryRegionMetaData()
                    {
                        AssetId = (int)(asset.Id),
                        RegionId = (int)(region.Id),
                        CountryId = (int)(region.CountryId)
                    });
                }
            }
            else
            {
                if (hasFolder)
                {
                    // check if folder has region metadata mapping, if yes, apply to asset as well
                    var folderRegions = _dbcontext.FolderCountryRegions.Include(fa => fa.Region).ThenInclude(r=>r.Country).Where(f => f.FolderId == asset.FolderId).ToList();

                    foreach (var region in folderRegions)
                    {
                        _dbcontext.AssetCountryRegions.Add(new AssetCountryRegionMetaData()
                        {
                            AssetId = (int)(asset.Id),
                            RegionId = (int)(region.RegionId),
                            CountryId = (int)(region.CountryId)
                        });
                        regionList.Add(region.Region.Description);

                        if (!countryList.Contains(region.Country.Name))
                        {
                            countryList.Add(region.Country.Name);
                        }
                    }
                }
            }
            _dbcontext.SaveChanges();

            // Insert into Audit Trail
            var assetAuditDetails = new AssetAuditDetailDto()
            {
                DisplayName = asset.Title,
                Description = null,
                Accounts = accountList,
                Countries = countryList,
                Regions = regionList,
                Folder = folderName,
                Tags = tagList,
                Expiry = null
            };

            var auditDisplayUser = _dbcontext.AppUsers.First(u => u.Id == request.UserId);

            var auditTrailEntry = new AssetAudit()
            {
                AssetId = Convert.ToInt32(asset.Id),
                AssetFileName = assetDto.FileName,
                AuditType = Convert.ToInt32(AssetAuditType.AssetUpload),
                AuditTypeText = AssetAuditType.AssetUpload.GetDescription(),
                AuditCreatedByUserId = asset.CreatedById,
                AuditCreatedDate = asset.CreatedDate,
                AuditCreatedByName = auditDisplayUser != null ? auditDisplayUser.UserName : "",
                NewParameters = _helperService.GetJsonString(assetAuditDetails),
                FolderId = asset.FolderId
            };

            _dbcontext.AssetAudit.Add(auditTrailEntry);
            _dbcontext.SaveChanges();

            result.Entity = GetAssetDetail(asset);
            result.ResultType = ResultType.Success;
            return result;
        }

        public override Exception OnException(Exception ex)
        {
            return new Exception(ex.Message, ex);
        }

        public AssetDto GetAssetDetail(Asset asset)
        {
            var result = _mapper.Map<AssetDto>(asset);
            result.Folder = null;
            var allVersions = _dbcontext.AssetVersions.ToList();
            var allTags = _dbcontext.Tags.ToList();
            var allUsers = _dbcontext.AppUsers.ToList();
            var allAccounts = _dbcontext.AssetAccounts.ToList();
            var allCountryRegions = _dbcontext.AssetCountryRegions.ToList();

            AssetVersionsDto version = new AssetVersionsDto();
            List<Tag> labels = new List<Tag>();
            result.AssetVersions = (List<AssetVersionsDto>)_mapper.Map<IEnumerable<AssetVersionsDto>>(allVersions.Where(a => a.AssetId == asset.Id));

            foreach (var ver in result.AssetVersions)
            {
                ver.CreatedByName = _dbcontext.AppUsers.First(u => u.Id == ver.CreatedById).UserName;
            }

            //get active version
            version = result.AssetVersions.Find((v) => v.ActiveVersion == 1);
            version.CreatedByName = allUsers.First(u => u.Id == version.CreatedById).UserName;
            //version = _dbcontext.AssetVersions.FirstOrDefault(a => a.AssetId == asset.Id && a.ActiveVersion == 1);
            labels = allTags.Where(a => a.AssetId == version.Id).ToList();
            result.Tags = (List<TagDto>)_mapper.Map<IEnumerable<TagDto>>(labels);
            //for the public share url, directly download
            result.DownloadUrl = version.DownloadUrl; //_azureStorageService.GetBlobSasUri(_configuration, string.Concat(version.Key, ".", version.Extension), version.FileName, false);
            result.Thumbnail = version.Thumbnail; //_azureStorageService.GetBlobSasUri(_configuration, string.Concat(version.Key, ".", version.Extension), string.Empty, true);

            result.CopyUrl = _configuration["BaseUrl"] + "Assets/" + version.Id;
            result.CreatedByName = allUsers.First(u => u.Id == asset.CreatedById).UserName;
            result.StatusName = ((AssetStatus)version.Status).GetDescription();
            if (!String.IsNullOrEmpty(asset.ModifiedById))
            {
                result.ModifiedByName = allUsers.First(u => u.Id == asset.ModifiedById).UserName;
            }

            result.Size = Convert.ToInt32(version.Size);
            result.Extension = version.Extension;
            result.FileName = version.FileName;
            result.FileSizeText = version.FileSizeText;
            result.FileType = version.FileType;
            result.Key = version.Key;
            result.OriginalUrl = version.OriginalUrl; 
            result.DownloadCount = version.DownloadCount;

            return result;
        }
    }
}