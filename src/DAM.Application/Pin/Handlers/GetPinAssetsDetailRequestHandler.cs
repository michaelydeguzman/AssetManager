using Microsoft.Extensions.Configuration;
using DAM.Application.Pin.Dtos;
using DAM.Persistence;
using System;
using System.Collections.Generic;
using System.Text;
using static DAM.Application.Pin.Requests.PinAssetRequest;
using System.Threading;
using System.Linq;
using AutoMapper;
using DAM.Application.Assets.Dtos;
using DAM.Domain.Entities;
using DAM.Application.Assets.Enums;
using DAM.Application.Extensions;

namespace DAM.Application.Pin.Handlers
{
    public class GetPinAssetsDetailRequestHandler : HandlerBase<GetPinAssetsDetailRequest, HandlerResult<IEnumerable<AssetDto>>>
    {
        private readonly IDbContext _dbcontext;
        private readonly IConfiguration _configuration;
        private readonly IMapper _mapper;

        public GetPinAssetsDetailRequestHandler(IDbContext dbcontext, IConfiguration configuration, IMapper mapper)
        {
            _dbcontext = dbcontext ?? throw new ArgumentNullException(nameof(dbcontext));
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        public override HandlerResult<IEnumerable<AssetDto>> HandleRequest(GetPinAssetsDetailRequest request, CancellationToken cancellationToken, HandlerResult<IEnumerable<AssetDto>> result)
        {
            var templist = _dbcontext.PinAssets.Where(a => a.UserId == request.UserId).ToList();
            if (templist.Count > 0)
            {
                var assetsList = new List<int>();
                templist.ForEach(p =>
                {
                    assetsList.Add(p.AssetId);
                });

                if (assetsList.Count > 0)
                {
                    List<Asset> assets = new List<Asset>();
                    assets = _dbcontext.Assets.Where(a => a.Status != Convert.ToInt32(AssetStatus.Archived) && a.Status != Convert.ToInt32(AssetStatus.Deleted) && assetsList.Contains(a.Id)).ToList();
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
                        labels = _dbcontext.Tags.Where(a => a.AssetId == version.Id).ToList();
                        asset.Tags = (List<TagDto>)_mapper.Map<IEnumerable<TagDto>>(labels);
                        //for the public share url, directly download
                        asset.DownloadUrl = version.DownloadUrl;
                        asset.Thumbnail = version.Thumbnail;
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
                        asset.OriginalUrl = version.OriginalUrl;
                        #endregion
                    }
                }
                result.ResultType = ResultType.Success;
            }
            else
            {
                result.ResultType = ResultType.NoData;
            }
            return result;
        }
    }
}
