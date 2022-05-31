using AutoMapper;
using DAM.Application.Assets.Dtos;
using DAM.Application.Assets.Requests;
using DAM.Application.Services.Interfaces;
using DAM.Domain.Entities;
using DAM.Persistence;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace DAM.Application.Assets.Handlers
{
    public class UpdateAssetTagsRequestHandler : HandlerBase<UpdateAssetTagsRequest, HandlerResult<List<AssetDto>>>
    {
        private readonly IDbContext _dbcontext;
        private readonly IMapper _mapper;

        public UpdateAssetTagsRequestHandler(IDbContext dbcontext, IMapper mapper)
        {
            _dbcontext = dbcontext ?? throw new ArgumentNullException(nameof(dbcontext));
        }

        public override HandlerResult<List<AssetDto>> HandleRequest(UpdateAssetTagsRequest request, CancellationToken cancellationToken, HandlerResult<List<AssetDto>> result)
        {
            var allTags = _dbcontext.Tags.ToList();

            foreach (var asset in request.AssetsToUpdate)
            {
                var currentVer = asset.AssetVersions.FirstOrDefault(av => av.ActiveVersion == 1);

                if (currentVer != null)
                {
                    var existingTags = allTags.Where(t => t.AssetId == currentVer.Id).ToList();

                    // remove tags whose id are not in list
                    var tagIds = asset.Tags.Where(at => at.Id.HasValue).Select(at => at.Id);
                    var removeTags = new List<Tag>();

                    foreach (var tag in existingTags)
                    {
                        if (!tagIds.Contains(tag.Id))
                        {
                            removeTags.Add(tag);
                        }
                    }

                    if (removeTags.Count > 0)
                    {
                        _dbcontext.Tags.RemoveRange(removeTags);
                        _dbcontext.SaveChanges();
                    }

                    // add tags without id
                    var addTags = asset.Tags.Where(at => at.Id == null).ToList();

                    var tagsToAdd = new List<Tag>();
                    foreach (var tag in addTags)
                    {
                        var newTag = new Tag
                        {
                            AssetId = currentVer.Id.Value,
                            Name = tag.Name,
                            IsCognitive = tag.IsCognitive
                        };
                        tagsToAdd.Add(newTag);
                    }

                    if (tagsToAdd.Count > 0)
                    {
                        _dbcontext.Tags.AddRange(tagsToAdd);
                        _dbcontext.SaveChanges();
                    }
                }
            }
            _dbcontext.SaveChanges();

            result.Entity = request.AssetsToUpdate;
            result.ResultType = ResultType.Success;
            return result;
        }
    }
}
