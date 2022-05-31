﻿using AutoMapper;
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
    public class GetImageAssetsRequestHandler : HandlerBase<GetImageAssetsRequest, HandlerResult<IEnumerable<AssetVersionsDto>>>
    {
        private readonly ICacheProvider _cacheProvider;
        private readonly IDbContext _dbcontext;
        private readonly IMapper _mapper;
        private readonly IConfiguration _configuration;
        private readonly IAzureStorageService _azureStorageService;

        public GetImageAssetsRequestHandler(ICacheProvider cacheProvider, IDbContext dbcontext, IMapper mapper, IConfiguration configuration, IAzureStorageService azureStorageService)
        {
            _cacheProvider = cacheProvider ?? throw new ArgumentNullException(nameof(cacheProvider));
            _dbcontext = dbcontext ?? throw new ArgumentNullException(nameof(dbcontext));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
            _azureStorageService = azureStorageService ?? throw new ArgumentNullException(nameof(azureStorageService));
        }

        public override HandlerResult<IEnumerable<AssetVersionsDto>> HandleRequest(GetImageAssetsRequest request, CancellationToken cancellationToken, HandlerResult<IEnumerable<AssetVersionsDto>> result)
        {
            List<AssetVersions> assetVersions;
            string[] imageExt = new string[] { "png", "jpg", "tif", "tiff", "jpeg", "bmp", "eps" };
            var approvalFlag = _dbcontext.FeatureFlags.First(x => x.FeatureFlagName == "Approvals");

            if (request.FolderIds.Count > 0)
            {
                var selectedFolderIds = request.FolderIds.ToArray();

                foreach (var folderId in selectedFolderIds)
                {
                    var subfolderIds = GetSubFolderIds(folderId);

                    foreach (var subfolderId in subfolderIds)
                    {
                        if (!selectedFolderIds.Contains(subfolderId))
                        {
                            request.FolderIds.Add(subfolderId);
                        }
                    }
                }

                if(approvalFlag.IsActivated)
                {
                    assetVersions = (from a in _dbcontext.Assets.ToList()
                                     join av in _dbcontext.AssetVersions.ToList()
                                     on a.Id equals av.AssetId
                                     join f in request.FolderIds
                                     on a.FolderId equals f
                                     where imageExt.Contains(av.Extension.ToLower()) && av.ActiveVersion == 1 && av.Status == (int)AssetStatus.Approved
                                     select av).ToList();
                }
                else
                {
                    assetVersions = (from a in _dbcontext.Assets.ToList()
                                     join av in _dbcontext.AssetVersions.ToList()
                                     on a.Id equals av.AssetId
                                     join f in request.FolderIds
                                     on a.FolderId equals f
                                     where imageExt.Contains(av.Extension.ToLower()) && av.ActiveVersion == 1 && av.Status != (int)AssetStatus.Archived && av.Status != (int)AssetStatus.Deleted
                                     select av).ToList();
                }
            }
            else
            {
                if(approvalFlag.IsActivated)
                {
                    assetVersions = (from a in _dbcontext.Assets.ToList()
                                     join av in _dbcontext.AssetVersions.ToList()
                                     on a.Id equals av.AssetId
                                     where imageExt.Contains(av.Extension.ToLower()) && av.ActiveVersion == 1 && av.Status == (int)AssetStatus.Approved
                                     select av).ToList();
                }
                else
                {
                    assetVersions = (from a in _dbcontext.Assets.ToList()
                                     join av in _dbcontext.AssetVersions.ToList()
                                     on a.Id equals av.AssetId
                                     where imageExt.Contains(av.Extension.ToLower()) && av.ActiveVersion == 1 && av.Status != (int)AssetStatus.Archived && av.Status != (int)AssetStatus.Deleted
                                     select av).ToList();
                }
                
            }
            result.Entity = _mapper.Map<IEnumerable<AssetVersionsDto>>(assetVersions);

            // Update Asset URLs
            foreach (var assetVersion in result.Entity)
            {
                //assetVersion.DownloadUrl = _azureStorageService.GetBlobSasUri(_configuration, string.Concat(assetVersion.Key, ".", assetVersion.Extension), assetVersion.FileName, false);
                //assetVersion.Thumbnail = _azureStorageService.GetBlobSasUri(_configuration, string.Concat(assetVersion.Key, ".", assetVersion.Extension), string.Empty, true);

                assetVersion.CreatedByName = _dbcontext.AppUsers.First(u => u.Id == assetVersion.CreatedById).UserName;

                // if (assetVersion.Extension == "tiff" || assetVersion.Extension == "tif")
                // {
                //     assetVersion.OriginalUrl = _azureStorageService.GetBlobSasUri(_configuration, Path.Combine("tiff", string.Concat(assetVersion.Key, ".", "png")), string.Empty, true);
                // }
                // else
                // {
                //     assetVersion.OriginalUrl = _azureStorageService.GetBlobSasUri(_configuration, string.Concat(assetVersion.Key, ".", assetVersion.Extension), string.Empty, false, true);
                // }
            }
            result.ResultType = ResultType.Success;
            return result;
        }

        private List<int> GetSubFolderIds(int folderId)
        {
            var subFolderIds = new List<int>();

            subFolderIds = _dbcontext.Folders.Where(x => x.ParentFolderId == folderId).Select(x => x.Id).ToList();

            var subFolderIdsCopy = subFolderIds.ToArray();

            foreach (var subFolderId in subFolderIdsCopy)
            {
                subFolderIds.AddRange(GetSubFolderIds(subFolderId));
            }

            return subFolderIds;
        }

        public override Exception OnException(Exception ex)
        {
            return new Exception(ex.Message, ex);
        }

    }
}