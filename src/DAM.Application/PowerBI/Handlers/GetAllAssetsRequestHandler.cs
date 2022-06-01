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
    public class GetAssetTableRequestHandler : HandlerBase<GetAssetTableRequest, HandlerResult<IEnumerable<Asset>>>//name need to match GetAssetTableRequest + Handler, than the HandlerBase need to match GetAssetTableRequest, and the HandlerResult need to match IEnumerable<Asset> which in ./Requests/PowerBIRequest.cs
    {
        private readonly IDbContext _dbcontext;
        private readonly IConfiguration _configuration;
        private readonly IAzureStorageService _azureStorageService;

        public GetAssetTableRequestHandler(IDbContext dbcontext, IConfiguration configuration, IAzureStorageService azureStorageService) //name is need to be same as this class name
        {
            _dbcontext = dbcontext ?? throw new ArgumentNullException(nameof(dbcontext));
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
            _azureStorageService = azureStorageService ?? throw new ArgumentNullException(nameof(azureStorageService));
        }

        public override HandlerResult<IEnumerable<Asset>> HandleRequest(GetAssetTableRequest request, CancellationToken cancellationToken, HandlerResult<IEnumerable<Asset>> result)//HandlerResult type need to be same as the class line 13 there are two handleresults in this line both of them need to be same as, also the request need to be same as class line 13
        {
            var delta = _dbcontext.Assets.ToList();
            result.Entity = delta;
            result.ResultType = ResultType.Success;
            return result;
        }
    }
}
