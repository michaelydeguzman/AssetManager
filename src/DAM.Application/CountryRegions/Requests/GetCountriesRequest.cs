using DAM.Application.CountryRegions.Dtos;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace DAM.Application.CountryRegions.Requests
{
    public class GetCountriesRequest : IRequest<HandlerResult<IEnumerable<CountryDto>>>
    {
        public GetCountriesRequest()
        {
        }
    }
}
