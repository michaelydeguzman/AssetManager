using DAM.Application.Companies.Dtos;
using MediatR;
using System.Collections.Generic;

namespace DAM.Application.Companies.Requests
{
    public class CompanyRequests : IRequest<HandlerResult<IEnumerable<CompanyDto>>>
    {
        public class GetCompaniesRequest : IRequest<HandlerResult<IEnumerable<CompanyDto>>>
        {
            public GetCompaniesRequest()
            {
            }
        }
        public class GetCompanyRequest : IRequest<HandlerResult<CompanyDto>>
        {
            public int PartnerId { get; set; }
            public GetCompanyRequest(int partnerId)
            {
                PartnerId = partnerId;
            }
        }

        public class AddCompanyRequest : IRequest<HandlerResult<CompanyDto>>
        {
            public string UserId { get; set; }
            public CompanyDto PartnerDto { get; set; }
            public AddCompanyRequest(CompanyDto partnerDto, string userId)
            {
                PartnerDto = partnerDto;
                UserId = userId;
            }
        }

        public class UpdateCompanyRequest : IRequest<HandlerResult<CompanyDto>>
        {
            public string UserId { get; set; }
            public CompanyDto CompanyDto { get; set; }
            public UpdateCompanyRequest(CompanyDto companyDto, string userId)
            {
                CompanyDto = companyDto;
                UserId = userId;
            }
        }
    }
}
