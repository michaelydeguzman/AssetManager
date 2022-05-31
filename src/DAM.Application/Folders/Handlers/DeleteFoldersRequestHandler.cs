using AutoMapper;
using DAM.Application.Assets.Helpers;
using DAM.Application.AuditTrail.Enums;

using DAM.Application.Cache;
using DAM.Application.Extensions;
using DAM.Application.Folders.Dtos;
using DAM.Domain.Entities;
using DAM.Persistence;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using static DAM.Application.Folders.Requests.FoldersRequest;

namespace DAM.Application.Folders.Handlers
{
    public class DeleteFoldersRequestHandler : HandlerBase<DeleteFolderRequest, HandlerResult<DeleteFoldersDto>>
    {
        private readonly ICacheProvider _cacheProvider;
        private readonly IDbContext _dbcontext;
        private readonly IMapper _mapper;

        public DeleteFoldersRequestHandler(ICacheProvider cacheProvider, IDbContext dbcontext, IMapper mapper)
        {
            _cacheProvider = cacheProvider ?? throw new ArgumentNullException(nameof(cacheProvider));
            _dbcontext = dbcontext ?? throw new ArgumentNullException(nameof(dbcontext));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        public override HandlerResult<DeleteFoldersDto> HandleRequest(DeleteFolderRequest request, CancellationToken cancellationToken, HandlerResult<DeleteFoldersDto> result)
        {
            var deletedBy = request.UserId;
            var folderIdsToDelete = request.DeleteFolders.FolderIds;
            result.Entity = new DeleteFoldersDto();
            result.Entity.IsSuccess = false;

            if(folderIdsToDelete.Count > 0)
            {
                var foldersToDelete = _dbcontext.Folders.Where(a => !a.Deleted && folderIdsToDelete.Contains((int)a.Id));
                var foldersDeleted = new List<int>();
                var auditTrail = new List<AssetAudit>();

                var auditDisplayUser = _dbcontext.AppUsers.First(u => u.Id == deletedBy);

                if (foldersToDelete.ToList().Count > 0)
                {
                    foreach (var folder in foldersToDelete)
                    {
                        folder.Deleted = true;
                        folder.ModifiedDate = DateTimeOffset.UtcNow;
                        folder.ModifiedById = deletedBy;
                        foldersDeleted.Add((int)folder.Id);

                        // Insert into Audit Trail
                        var auditTrailEntry = new AssetAudit()
                        {
                            FolderId = Convert.ToInt32(folder.Id),
                            FolderName = folder.FolderName,
                            AuditType = Convert.ToInt32(AssetAuditType.FolderDeleted),
                            AuditTypeText = AssetAuditType.FolderDeleted.GetDescription(),
                            AuditCreatedByUserId = deletedBy,
                            AuditCreatedDate = DateTimeOffset.UtcNow,
                            AuditCreatedByName = auditDisplayUser != null ? auditDisplayUser.UserName : ""
                        };

                        auditTrail.Add(auditTrailEntry);
                    }

                    _dbcontext.Folders.UpdateRange(foldersToDelete);
                    _dbcontext.AssetAudit.AddRange(auditTrail);
                    _dbcontext.SaveChanges();

                    result.Entity.FolderIds = foldersDeleted;
                    result.Entity.IsSuccess = true;
              
                    _dbcontext.SaveChanges();
                }
            }
            result.ResultType = ResultType.Success;
            return result;
        }
        public override Exception OnException(Exception ex)
        {
            return new Exception(ex.Message, ex);
        }
    }
}
