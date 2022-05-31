using AutoMapper;
using AutoMapper.Configuration;
using DAM.Application.Cache;
using DAM.Application.FeatureFlags.Dtos;
using DAM.Application.FeatureFlags.Requests;
using DAM.Application.Companies.Dtos;
using DAM.Application.UserRoles.Dtos;
using DAM.Domain.Entities;
using DAM.Domain.Entities.Identity;
using DAM.Persistence;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using static DAM.Application.Companies.Requests.CompanyRequests;

namespace DAM.Application.UserRoles.Handlers
{
    public class GetCompaniesRequestHandler : HandlerBase<GetCompaniesRequest, HandlerResult<IEnumerable<CompanyDto>>>
    {
        private readonly IDbContext _dbcontext;
        private readonly IMapper _mapper;

        public GetCompaniesRequestHandler(IDbContext dbcontext, IMapper mapper)
        {
            _dbcontext = dbcontext ?? throw new ArgumentNullException(nameof(dbcontext));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        public override HandlerResult<IEnumerable<CompanyDto>> HandleRequest(GetCompaniesRequest request, CancellationToken cancellationToken, HandlerResult<IEnumerable<CompanyDto>> result)
        {
            var partners = new List<Company>();
            var users = new List<ApplicationUser>();
                
            partners = _dbcontext.Companies.ToList();
            var partnerDto = _mapper.Map<IEnumerable<CompanyDto>>(partners);
            foreach (var partner in partnerDto)
            {
                partner.RootFolderName = _dbcontext.Folders.Where(f => f.Id == partner.RootFolderId).FirstOrDefault().FolderName;
            }
            result.Entity = partnerDto;
            result.ResultType = ResultType.Success;

            return result;
        } 

        public override Exception OnException(Exception ex)
        {
            return new Exception(ex.Message, ex);
        }
    }
}
