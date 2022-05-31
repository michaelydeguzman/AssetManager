using AutoMapper;
using DAM.Application.Approvals.Dtos;
using DAM.Application.Cache;
using DAM.Persistence;
using DAM.Domain.Entities;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Linq;
using DAM.Application.Approvals.Requests;
using DAM.Application.Approval.Dtos;
using DAM.Application.Assets.Enums;
using Microsoft.EntityFrameworkCore;
using DAM.Application.Assets.Dtos;
using DAM.Application.Extensions;

namespace DAM.Application.Approvals.Handlers
{
    public class GetMySentApprovalAssetsRequestHandler : HandlerBase<GetMySentApprovalAssetsRequest, HandlerResult<IEnumerable<AssetDto>>>
    {
        private readonly ICacheProvider _cacheProvider;
        private readonly IDbContext _dbcontext;
        private readonly IMapper _mapper;
        private readonly IConfiguration _configuration;

        public GetMySentApprovalAssetsRequestHandler(ICacheProvider cacheProvider, IDbContext dbcontext, IMapper mapper, IConfiguration configuration)
        {
            _cacheProvider = cacheProvider ?? throw new ArgumentNullException(nameof(cacheProvider));
            _dbcontext = dbcontext ?? throw new ArgumentNullException(nameof(dbcontext));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
        }

        public override HandlerResult<IEnumerable<AssetDto>> HandleRequest(GetMySentApprovalAssetsRequest request, CancellationToken cancellationToken, HandlerResult<IEnumerable<AssetDto>> result)
        {
            var templist = _dbcontext.ApprovalLevels.Where(a => a.CreatedById == request.UserId).ToList();

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
                    assets = _dbcontext.Assets.Where(a => a.Status != Convert.ToInt32(AssetStatus.Archived) && a.Status != Convert.ToInt32(AssetStatus.Deleted) && a.Status != Convert.ToInt32(AssetStatus.Draft) && assetsList.Contains(a.Id)).ToList();
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

        public override Exception OnException(Exception ex)
        {
            return new Exception(ex.Message, ex);
        }
    }
}
