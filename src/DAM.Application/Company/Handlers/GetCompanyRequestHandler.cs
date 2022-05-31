using AutoMapper;
using DAM.Application.Companies.Dtos;
using DAM.Domain.Entities;
using DAM.Persistence;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using static DAM.Application.Companies.Requests.CompanyRequests;

namespace DAM.Application.Companies.Handlers
{
    public class GetCompanyRequestHandler : HandlerBase<GetCompanyRequest, HandlerResult<CompanyDto>>
    {
        private readonly IDbContext _dbcontext;
        private readonly IMapper _mapper;
        private readonly IConfiguration _configuration;

        public GetCompanyRequestHandler(IDbContext dbcontext, IMapper mapper, IConfiguration configuration)
        {
            _dbcontext = dbcontext ?? throw new ArgumentNullException(nameof(dbcontext));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
        }

        public override HandlerResult<CompanyDto> HandleRequest(GetCompanyRequest request, CancellationToken cancellationToken, HandlerResult<CompanyDto> result)
        {
            var partner = new Company();

            if(request.PartnerId > 0)
            {
                partner = _dbcontext.Companies.Where(p => p.Id == request.PartnerId).FirstOrDefault();
            }

            result.Entity = _mapper.Map<CompanyDto>(partner);
            result.ResultType = ResultType.Success;
            return result;
        }

        public override Exception OnException(Exception ex)
        {
            return new Exception(ex.Message, ex);
        }
    }
}
