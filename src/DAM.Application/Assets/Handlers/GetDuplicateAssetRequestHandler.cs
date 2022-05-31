using AutoMapper;
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
using DAM.Application.Extensions;
using System.IO;
using Force.Crc32;

namespace DAM.Application.Assets.Handlers
{
    public class GetDuplicateAssetRequestHandler : HandlerBase<GetDuplicateAssetRequest, HandlerResult<AssetDto>>
    {
        private readonly IDbContext _dbcontext;
        private readonly IMapper _mapper;
        private readonly IConfiguration _configuration;

        public GetDuplicateAssetRequestHandler(ICacheProvider cacheProvider, IDbContext dbcontext, IMapper mapper, IConfiguration configuration, IAzureStorageService azureStorageService)
        {
            _dbcontext = dbcontext ?? throw new ArgumentNullException(nameof(dbcontext));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
        }

        public override HandlerResult<AssetDto> HandleRequest(GetDuplicateAssetRequest request, CancellationToken cancellationToken, HandlerResult<AssetDto> result)
        {
            using (Stream originalStream = new MemoryStream(request.Asset.FileBytes))
            {
                byte[] bytes = new byte[originalStream.Length];
                uint crc = Crc32Algorithm.Compute(bytes);

                var allAssets = _dbcontext.Assets.ToList();

                var versions = _dbcontext.AssetVersions.Where(a => a.Status != Convert.ToInt32(AssetStatus.Archived) && a.Status != Convert.ToInt32(AssetStatus.Deleted) && a.CRC32Code == crc).ToList();

                if (versions != null)
                {
                    foreach (var version in versions)
                    {
                        var asset = allAssets.FirstOrDefault(a => a.Status != Convert.ToInt32(AssetStatus.Archived) && a.Status != Convert.ToInt32(AssetStatus.Deleted) && a.Id == version.AssetId);
                        if(asset != null)
                        {
                            var duplicateAsset = _mapper.Map<AssetDto>(asset);
                            var duplicateVersion = _mapper.Map<AssetVersionsDto>(version);
                            duplicateAsset.AssetVersions.Add(duplicateVersion);
                            duplicateAsset.Size = Convert.ToInt32(duplicateVersion.Size);
                            duplicateAsset.Extension = duplicateVersion.Extension;
                            duplicateAsset.FileName = duplicateVersion.FileName;
                            duplicateAsset.FileSizeText = duplicateVersion.FileSizeText;
                            duplicateAsset.FileType = duplicateVersion.FileType;
                            duplicateAsset.Key = duplicateVersion.Key;
                            result.Entity = duplicateAsset;
                            result.ResultType = ResultType.Success;
                            return result;
                        }
                        
                    }
                }
            }
            result.ResultType = ResultType.NoData;
            return result;
        }

        public override Exception OnException(Exception ex)
        {
            return new Exception(ex.Message, ex);
        }

    }
}