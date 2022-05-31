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
using DAM.Application.Assets.Dtos;
using DAM.Application.Approvals.Enums;
using DAM.Application.Assets.Enums;
using DAM.Application.Services.Interfaces;

namespace DAM.Application.Approvals.Handlers
{
    public class GetAssetsForApprovalRequestHandler : HandlerBase<GetAssetsForApprovalRequest, HandlerResult<IEnumerable<AssetDto>>>
    {
        private readonly ICacheProvider _cacheProvider;
        private readonly IDbContext _dbcontext;
        private readonly IMapper _mapper;
        private readonly IConfiguration _configuration;
        private readonly IAzureStorageService _azureStorageService;


        public GetAssetsForApprovalRequestHandler(ICacheProvider cacheProvider, IDbContext dbcontext, IMapper mapper, IConfiguration configuration, IAzureStorageService azureStorageService)
        {
            _cacheProvider = cacheProvider ?? throw new ArgumentNullException(nameof(cacheProvider));
            _dbcontext = dbcontext ?? throw new ArgumentNullException(nameof(dbcontext));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
            _azureStorageService = azureStorageService ?? throw new ArgumentNullException(nameof(azureStorageService));
        }

        public override HandlerResult<IEnumerable<AssetDto>> HandleRequest(GetAssetsForApprovalRequest request, CancellationToken cancellationToken, HandlerResult<IEnumerable<AssetDto>> result)
        {
            var allTags = _dbcontext.Tags.ToList();
            var allVersions = _dbcontext.AssetVersions.ToList();
            var assetsForApproval = (from userApprovals in _dbcontext.ApprovalLevelApprovers
                                     join assetApprovalLevels in _dbcontext.ApprovalLevels on userApprovals.ApprovalLevelId equals assetApprovalLevels.Id
                                     join asset in _dbcontext.Assets on assetApprovalLevels.AssetId equals asset.Id
                                     where userApprovals.ApproverId == request.UserId && userApprovals.ApprovalStatus == (int)ApprovalStatus.Pending
                                     && userApprovals.ReviewDate == null
                                     && assetApprovalLevels.IsActiveLevel == true && assetApprovalLevels.CompletedDate == null
                                     && asset.Status == (int)AssetStatus.Submitted
                                     select asset).Distinct();

            result.Entity = _mapper.Map<IEnumerable<AssetDto>>(assetsForApproval);
            result.ResultType = ResultType.Success;
            // Update Asset URLs
            foreach (var asset in result.Entity)
            {
                var version = _dbcontext.AssetVersions.FirstOrDefault(a => a.AssetId == asset.Id && a.ActiveVersion == 1);
                var activeApprovalLevel = _dbcontext.ApprovalLevels.FirstOrDefault(x => x.IsActiveLevel.Value == true && x.AssetId == asset.Id && x.AssetVersionId == version.Id);
                var creator = _dbcontext.AppUsers.First(u => u.Id == asset.CreatedById);
                asset.Thumbnail = version.Thumbnail; //_azureStorageService.GetBlobSasUri(_configuration, string.Concat(version.Key, ".", version.Extension), string.Empty, true);
                asset.OriginalUrl = version.OriginalUrl; //_azureStorageService.GetBlobSasUri(_configuration, string.Concat(version.Key, ".", version.Extension), string.Empty, false, true);
                asset.CreatedByName = creator.UserName;
                asset.Extension = version.Extension;
                asset.FileType = version.FileType;
                asset.Size = Convert.ToInt32(version.Size);
                asset.FileName = version.FileName;
                asset.FileSizeText = version.FileSizeText;
                asset.Key = version.Key;

                if (activeApprovalLevel != null)
                {
                    asset.DueDate = activeApprovalLevel.DueDate;
                    asset.CurrentApprovalLevelNumber = activeApprovalLevel.LevelNumber.ToString();
                }
                asset.AssetVersions = (List<AssetVersionsDto>)_mapper.Map<IEnumerable<AssetVersionsDto>>(allVersions.Where(a => a.AssetId == asset.Id));
                asset.Tags = (List<TagDto>)_mapper.Map<IEnumerable<TagDto>>(allTags.Where(a => a.AssetId == version.Id).ToList());

                asset.OwnerProfilePic = _azureStorageService.GetBlobSasUri(_configuration, string.Concat(creator.Id, ".", creator.ImageFileExtension));

                var folder = _dbcontext.Folders.FirstOrDefault(f => f.Id == asset.FolderId);
                asset.FolderName = string.Empty;
                if (folder != null)
                {
                    asset.FolderName = folder.FolderName;
                }
            }
            return result;
        }

        public override Exception OnException(Exception ex)
        {
            return new Exception(ex.Message, ex);
        }
    }
}
