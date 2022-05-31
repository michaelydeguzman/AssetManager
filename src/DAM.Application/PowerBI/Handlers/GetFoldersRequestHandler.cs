using DAM.Persistence;
using System;
using Microsoft.Extensions.Configuration;
using DAM.Application.Services.Interfaces;
using System.Threading;
using DAM.Domain.Entities;
using static DAM.Application.PwoerBI.Requests.PowerBIRequest;
using System.Collections.Generic;
using System.Linq;

namespace DAM.Application.PwoerBI.Handlers
{
    public class GetFoldersTableRequestHandler : HandlerBase<GetFoldersTableRequest, HandlerResult<IEnumerable<Folder>>>
    {
        private readonly IDbContext _dbcontext;
        private readonly IConfiguration _configuration;
        private readonly IAzureStorageService _azureStorageService;

        public GetFoldersTableRequestHandler(IDbContext dbcontext, IConfiguration configuration, IAzureStorageService azureStorageService)
        {
            _dbcontext = dbcontext ?? throw new ArgumentNullException(nameof(dbcontext));
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
            _azureStorageService = azureStorageService ?? throw new ArgumentNullException(nameof(azureStorageService));
        }

        public override HandlerResult<IEnumerable<Folder>> HandleRequest(GetFoldersTableRequest request, CancellationToken cancellationToken, HandlerResult<IEnumerable<Folder>> result)
        {
            var delta = _dbcontext.Folders.ToList();
            result.Entity = delta;
            result.ResultType = ResultType.Success;
            return result;
        }
    }
}
