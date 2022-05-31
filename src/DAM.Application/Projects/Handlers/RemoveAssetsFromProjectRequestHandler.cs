using System;
using System.Threading;
using AutoMapper;
using DAM.Persistence;
using DAM.Application.Projects.Requests;
using DAM.Application.Projects.Dtos;
using DAM.Domain.Entities;
using System.Collections.Generic;
using System.Linq;

namespace DAM.Application.Projects.Handlers
{
    public class RemoveAssetsFromProjectRequestHandler : HandlerBase<RemoveAssetsFromProjectRequest, HandlerResult<ImportAssetsToProjectDto>>
    {
        private readonly IDbContext _dbcontext;
        private readonly IMapper _mapper;

        public RemoveAssetsFromProjectRequestHandler(IMapper mapper, IDbContext dbcontext)
        {
            _dbcontext = dbcontext ?? throw new ArgumentNullException(nameof(dbcontext));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        public override HandlerResult<ImportAssetsToProjectDto> HandleRequest(RemoveAssetsFromProjectRequest request, 
            CancellationToken cancellationToken, HandlerResult<ImportAssetsToProjectDto> result)
        {
            var assetIds = request.AssetsToRemove.AssetIds;
            var projectIds = request.AssetsToRemove.ProjectIds;
            var allProjectItems = _dbcontext.ProjectItems.ToList();
            var allProjectAssets = _dbcontext.Assets.Where(a=>a.ProjectId.HasValue).ToList();

            var projectItemsToDelete = new List<ProjectItem>();

            var projectAssetsToRemove = new List<Asset>();

            foreach (var projectId in projectIds)
            {
                foreach(var assetId in assetIds)
                {
                    var projectItem = allProjectItems.FirstOrDefault(pi => pi.AssetId == assetId && pi.ProjectId == projectId);

                    if (projectItem != null)
                    {
                        projectItemsToDelete.Add(projectItem);
                    }

                    var projectAsset = allProjectAssets.FirstOrDefault(pa => pa.Id == assetId && pa.ProjectId == projectId);

                    if (projectAsset != null)
                    {
                        projectAsset.ProjectId = null;
                        projectAssetsToRemove.Add(projectAsset);
                    }
                }
            }

            if (projectItemsToDelete.Count > 0)
            {
                _dbcontext.ProjectItems.RemoveRange(projectItemsToDelete);
                _dbcontext.SaveChanges();
            }

            if (projectAssetsToRemove.Count > 0)
            {
                _dbcontext.Assets.UpdateRange(projectAssetsToRemove);
                _dbcontext.SaveChanges();
            }

            result.Entity = request.AssetsToRemove;
            result.ResultType = ResultType.Success;
            return result;
        }
    }
}
