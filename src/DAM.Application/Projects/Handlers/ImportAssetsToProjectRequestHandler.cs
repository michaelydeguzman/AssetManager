using System;
using System.Threading;
using AutoMapper;
using DAM.Persistence;
using DAM.Application.Projects.Requests;
using DAM.Application.Projects.Dtos;
using DAM.Domain.Entities;
using System.Collections.Generic;

namespace DAM.Application.Projects.Handlers
{
    public class ImportAssetsToProjectRequestHandler : HandlerBase<ImportAssetsToProjectRequest, HandlerResult<ImportAssetsToProjectDto>>
    {
        private readonly IDbContext _dbcontext;
        private readonly IMapper _mapper;

        public ImportAssetsToProjectRequestHandler(IMapper mapper, IDbContext dbcontext)
        {
            _dbcontext = dbcontext ?? throw new ArgumentNullException(nameof(dbcontext));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        public override HandlerResult<ImportAssetsToProjectDto> HandleRequest(ImportAssetsToProjectRequest request, CancellationToken cancellationToken, HandlerResult<ImportAssetsToProjectDto> result)
        {
            var assetIds = request.Imports.AssetIds;
            var projectIds = request.Imports.ProjectIds;

            var newProjectItems = new List<ProjectItem>();
            foreach(var projectId in projectIds)
            {
                foreach(var assetId in assetIds)
                {
                    var newProjectAsset = new ProjectItem()
                    {
                        ProjectId = projectId,
                        AssetId = assetId,
                        CreatedById = request.UserId
                    };

                    newProjectItems.Add(newProjectAsset);
                }
            }

            if (newProjectItems.Count > 0)
            {
                _dbcontext.ProjectItems.AddRange(newProjectItems);
                _dbcontext.SaveChanges();
            }

            result.Entity = request.Imports;
            result.ResultType = ResultType.Success;
            return result;
        }
    }
}
