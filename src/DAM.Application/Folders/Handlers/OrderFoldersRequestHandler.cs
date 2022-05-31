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
    public class OrderFoldersRequestHandler : HandlerBase<OrderFoldersRequest, HandlerResult<List<MoveFolderDto>>>
    {
        private readonly ICacheProvider _cacheProvider;
        private readonly IDbContext _dbcontext;
        private readonly IMapper _mapper;
        private readonly IConfiguration _configuration;

        public OrderFoldersRequestHandler(ICacheProvider cacheProvider, IDbContext dbcontext, IMapper mapper, IConfiguration configuration)
        {
            _cacheProvider = cacheProvider ?? throw new ArgumentNullException(nameof(cacheProvider));
            _dbcontext = dbcontext ?? throw new ArgumentNullException(nameof(dbcontext));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
        }

        public override HandlerResult<List<MoveFolderDto>> HandleRequest(OrderFoldersRequest request, CancellationToken cancellationToken, HandlerResult<List<MoveFolderDto>> result)
        {
            if(request.Folders.Count > 0)
            {
                request.Folders.ForEach(f =>
                {
                    var newOrderNumber = f.OrderNumber;
                    var modifiedBy = request.UserId;

                    // If modifiedBy is null, default user
                    if (string.IsNullOrEmpty(modifiedBy))
                    {
                        modifiedBy = _dbcontext.AppUsers.First(u => u.Email == _configuration["DefaultUser"]).Id;
                    }

                    var folder = _dbcontext.Folders.First(a => a.Id == f.FolderId);

                    if (folder != null)
                    {
                        var oldFolder = _dbcontext.Folders.First(f => f.Id == folder.ParentFolderId).FolderName;

                        folder.OrderNumber = newOrderNumber;
                        folder.ModifiedDate = DateTimeOffset.UtcNow;
                        folder.ModifiedById = modifiedBy;
                        _dbcontext.Folders.Update(folder);
                    }
                });
                _dbcontext.SaveChanges();
            }
            result.Entity = request.Folders;
            result.ResultType = ResultType.Success;
            return result;
        }

        public override Exception OnException(Exception ex)
        {
            return new Exception(ex.Message, ex);
        }

    }
}
