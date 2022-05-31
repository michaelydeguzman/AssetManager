using DAM.Application.Accounts.Dtos;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace DAM.Application.Accounts.Requests
{
    public class GetAccountsRequest : IRequest<HandlerResult<IEnumerable<AccountDto>>>
    {
        public GetAccountsRequest()
        {
        }
    }
}
