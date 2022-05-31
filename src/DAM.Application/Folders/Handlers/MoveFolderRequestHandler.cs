using AutoMapper;
using DAM.Application.AuditTrail.Enums;

using DAM.Application.Cache;
using DAM.Application.Extensions;
using DAM.Application.Folders.Dtos;
using DAM.Domain.Entities;
using DAM.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using static DAM.Application.Folders.Requests.FoldersRequest;

namespace DAM.Application.Folders.Handlers
{
    public class MoveFolderRequestHandler : HandlerBase<MoveFolderRequest, HandlerResult<MoveFolderDto>>
    {
        private readonly ICacheProvider _cacheProvider;
        private readonly IDbContext _dbcontext;
        private readonly IMapper _mapper;
        private readonly IConfiguration _configuration;

        public MoveFolderRequestHandler(ICacheProvider cacheProvider, IDbContext dbcontext, IMapper mapper, IConfiguration configuration)
        {
            _cacheProvider = cacheProvider ?? throw new ArgumentNullException(nameof(cacheProvider));
            _dbcontext = dbcontext ?? throw new ArgumentNullException(nameof(dbcontext));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
        }

        public override HandlerResult<MoveFolderDto> HandleRequest(MoveFolderRequest request, CancellationToken cancellationToken, HandlerResult<MoveFolderDto> result)
        {
            var newParentFolderId = request.MoveFolderDto.ParentFolderId;
            var modifiedBy = request.UserId;

            // If modifiedBy is null, default user
            if (string.IsNullOrEmpty(modifiedBy))
            {
                modifiedBy = _dbcontext.AppUsers.First(u => u.Email == _configuration["DefaultUser"]).Id;
            }

            var folder = _dbcontext.Folders.First(a => a.Id == request.MoveFolderDto.FolderId);

            if (folder != null)
            {
                var oldFolder = _dbcontext.Folders.First(f => f.Id == folder.ParentFolderId).FolderName;
                
                folder.ParentFolderId = newParentFolderId == 0 ? null : newParentFolderId;
                folder.ModifiedDate = DateTimeOffset.UtcNow;
                folder.ModifiedById = modifiedBy;
                _dbcontext.Folders.Update(folder);

                // Insert into Audit Trail
                var auditDisplayUser = _dbcontext.AppUsers.First(u => u.Id == modifiedBy);
               
                var newFolder = _dbcontext.Folders.First(f => f.Id == newParentFolderId).FolderName;

                var auditTrailEntry = new AssetAudit()
                {
                    FolderId = Convert.ToInt32(folder.Id),
                    FolderName = folder.FolderName,
                    AuditType = Convert.ToInt32(AssetAuditType.FolderMoved),
                    AuditTypeText = AssetAuditType.FolderMoved.GetDescription(),
                    AuditCreatedByUserId = modifiedBy,
                    AuditCreatedDate = DateTimeOffset.UtcNow,
                    AuditCreatedByName = auditDisplayUser != null ? auditDisplayUser.UserName : "",
                    PreviousParameters = $"Folder: {oldFolder}",
                    NewParameters = $"Folder: {newFolder}"
                };

                _dbcontext.Add(auditTrailEntry);
                _dbcontext.SaveChanges();
            }
          
            result.Entity = request.MoveFolderDto;
            result.ResultType = ResultType.Success;

            return result;
        }

        public override Exception OnException(Exception ex)
        {
            return new Exception(ex.Message, ex);
        }

    }
}
