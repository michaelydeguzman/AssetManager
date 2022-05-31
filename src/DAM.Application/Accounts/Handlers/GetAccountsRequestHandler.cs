using AutoMapper;
using DAM.Application.Accounts.Dtos;
using DAM.Application.Accounts.Requests;
using DAM.Application.Cache;
using DAM.Domain.Entities;
using DAM.Persistence;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace DAM.Application.Accounts.Handlers
{
    public class GetAccountsRequestHandler : HandlerBase<GetAccountsRequest, HandlerResult<IEnumerable<AccountDto>>>
    {
        private readonly ICacheProvider _cacheProvider;
        private readonly IDbContext _dbcontext;
        private readonly IMapper _mapper;

        public GetAccountsRequestHandler(ICacheProvider cacheProvider, IDbContext dbcontext, IMapper mapper)
        {
            _cacheProvider = cacheProvider ?? throw new ArgumentNullException(nameof(cacheProvider));
            _dbcontext = dbcontext ?? throw new ArgumentNullException(nameof(dbcontext));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        public override HandlerResult<IEnumerable<AccountDto>> HandleRequest(GetAccountsRequest request, CancellationToken cancellationToken, HandlerResult<IEnumerable<AccountDto>> result)
        {
            var accounts = _dbcontext.Accounts.ToList();

            //var allAssetAccounts = _dbcontext.AssetAccounts.ToList();
            //var obj = (List<AccountDto>)_mapper.Map<IEnumerable<AccountDto>>(allAssetAccounts);
            //_cacheProvider.Save("allAssetAccounts", obj);

            result.Entity = _mapper.Map<IEnumerable<AccountDto>>(accounts);
            result.ResultType = ResultType.Success;

            return result;
        }

        public override Exception OnException(Exception ex)
        {
            return new Exception(ex.Message, ex);
        }
    }
}
