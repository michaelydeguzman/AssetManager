using AutoMapper;
using DAM.Application.Cache;
using DAM.Application.CountryRegions.Dtos;
using DAM.Application.CountryRegions.Requests;
using DAM.Domain.Entities;
using DAM.Persistence;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace DAM.Application.CountryRegions.Handlers
{
    public class GetCountriesRequestHandler : HandlerBase<GetCountriesRequest, HandlerResult<IEnumerable<CountryDto>>>
    {
        private readonly ICacheProvider _cacheProvider;
        private readonly IDbContext _dbcontext;
        private readonly IMapper _mapper;

        public GetCountriesRequestHandler(ICacheProvider cacheProvider, IDbContext dbcontext, IMapper mapper)
        {
            _cacheProvider = cacheProvider ?? throw new ArgumentNullException(nameof(cacheProvider));
            _dbcontext = dbcontext ?? throw new ArgumentNullException(nameof(dbcontext));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        public override HandlerResult<IEnumerable<CountryDto>> HandleRequest(GetCountriesRequest request, CancellationToken cancellationToken, HandlerResult<IEnumerable<CountryDto>> result)
        {
            List<Country> countries = new List<Country>();
            countries = _dbcontext.Countries
                .Include(c => c.Regions)
                .ToList();

            //var countryRegions = _dbcontext.AssetCountryRegions.ToList();
            //var allCountries = (List<CountryDto>)_mapper.Map<IEnumerable<CountryDto>>(countryRegions);
            //var allRegions = (List<RegionDto>)_mapper.Map<IEnumerable<RegionDto>>(countryRegions);
            //_cacheProvider.Save("allCountries", allCountries);
            //_cacheProvider.Save("allRegions", allRegions);

            result.Entity = _mapper.Map<IEnumerable<CountryDto>>(countries);
            result.ResultType = ResultType.Success;
            return result;
        }

        public override Exception OnException(Exception ex)
        {
            return new Exception(ex.Message, ex);
        }
    }
}