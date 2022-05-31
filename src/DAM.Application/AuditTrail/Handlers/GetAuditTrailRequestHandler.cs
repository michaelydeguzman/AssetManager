using AutoMapper;
using AutoMapper.Configuration;
using DAM.Application.AuditTrail.Dtos;
using DAM.Application.AuditTrail.Enums;
using DAM.Application.AuditTrail.Requests;
using DAM.Application.Cache;
using DAM.Domain.Entities;
using DAM.Persistence;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace DAM.Application.AuditTrail.Handlers
{
    public class GetAuditTrailRequestHandler : HandlerBase<GetAuditTrailRequest, HandlerResult<AuditTrailDto>>
    {
        private readonly ICacheProvider _cacheProvider;
        private readonly IDbContext _dbcontext;
        private readonly IMapper _mapper;

        public GetAuditTrailRequestHandler(ICacheProvider cacheProvider, IDbContext dbcontext, IMapper mapper)
        {
            _cacheProvider = cacheProvider ?? throw new ArgumentNullException(nameof(cacheProvider));
            _dbcontext = dbcontext ?? throw new ArgumentNullException(nameof(dbcontext));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        public override HandlerResult<AuditTrailDto> HandleRequest(GetAuditTrailRequest request, CancellationToken cancellationToken, HandlerResult<AuditTrailDto> result)
        {
            var auditTrailData = new List<AssetAudit>();

            var auditType = request.FetchAuditParams.AuditType;
            var fileName = request.FetchAuditParams.FileName;
            var folderName = request.FetchAuditParams.FolderName;

            // implement sorting by column
            switch (request.FetchAuditParams.SortColumnName.ToLower())
            {
                case "auditcreateddate":
                    auditTrailData = request.FetchAuditParams.SortOrder == (int)SortOrder.Ascending ? _dbcontext.AssetAudit.OrderBy(x => x.AuditCreatedDate).ToList()
                : _dbcontext.AssetAudit.OrderByDescending(x => x.AuditCreatedDate).ToList();
                    break;
                case "auditcreatedname":
                    auditTrailData = request.FetchAuditParams.SortOrder == (int)SortOrder.Ascending ? _dbcontext.AssetAudit.OrderBy(x => x.AuditCreatedByName).ToList()
              : _dbcontext.AssetAudit.OrderByDescending(x => x.AuditCreatedByName).ToList();
                    break;
                default:
                    auditTrailData = request.FetchAuditParams.SortOrder == (int)SortOrder.Ascending ? _dbcontext.AssetAudit.OrderBy(x => x.Id).ToList()
              : _dbcontext.AssetAudit.OrderByDescending(x => x.Id).ToList();
                    break;
            }

            var filteredAuditTrailData = auditTrailData;

            // implement filtering by column
            //if (auditType != null)
            //{
            //    filteredAuditTrailData = auditTrailData.Where(a => a.AuditType == Convert.ToInt32(auditType)).ToList();
            //}

            if(!String.IsNullOrEmpty(fileName))
            {
                filteredAuditTrailData = filteredAuditTrailData.Where(a => a.AssetFileName == fileName).ToList();
            }

            if(!String.IsNullOrEmpty(folderName))
            {
                foreach (var audit in filteredAuditTrailData)
                {
                    if (audit.FolderId == null)
                    {
                        var asset = _dbcontext.Assets.FirstOrDefault(x => x.Id == audit.AssetId);
                        if (asset != null)
                        {
                            if (asset.FolderId.HasValue)
                            {
                                var folder = _dbcontext.Folders.FirstOrDefault(x => x.Id == asset.FolderId.Value);

                                if (folder != null)
                                {
                                    audit.FolderName = folder.FolderName;
                                }
                            }
                        }
                    }
                }

                filteredAuditTrailData = filteredAuditTrailData.Where(a => a.FolderName == folderName).ToList();
            }

            var assetAuditTrail = filteredAuditTrailData
                    .Skip((request.FetchAuditParams.CurrentPageNumber - 1) * request.FetchAuditParams.PageSize)
                    .Take(request.FetchAuditParams.PageSize)
                    .ToList();

            if(String.IsNullOrEmpty(folderName))
            {
                foreach (var audit in assetAuditTrail)
                {
                    if (audit.FolderId == null)
                    {
                        var asset = _dbcontext.Assets.FirstOrDefault(x => x.Id == audit.AssetId);
                        if (asset != null)
                        {
                            if (asset.FolderId.HasValue)
                            {
                                var folder = _dbcontext.Folders.FirstOrDefault(x => x.Id == asset.FolderId.Value);

                                if (folder != null)
                                {
                                    audit.FolderName = folder.FolderName;
                                }
                            }
                        }
                    }
                    else
                    {
                        audit.FolderName = _dbcontext.Folders.FirstOrDefault(x => x.Id == audit.FolderId).FolderName;
                    }
                }
            }

            result.Entity = new AuditTrailDto()
            {
                TotalCount = filteredAuditTrailData.Count,
                AssetAuditTrail = _mapper.Map<IEnumerable<AssetAuditTrailDto>>(assetAuditTrail).ToList()
            };
            result.ResultType = ResultType.Success;
            return result;
        }

        public override Exception OnException(Exception ex)
        {
            return new Exception(ex.Message, ex);
        }

    }
}
