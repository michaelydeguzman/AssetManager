
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
    public class UpdateCompanyRequestHandler : HandlerBase<UpdateCompanyRequest, HandlerResult<CompanyDto>>
    {
        private readonly IDbContext _dbcontext;
        private readonly IMapper _mapper;
        private readonly IConfiguration _configuration;

        public UpdateCompanyRequestHandler(IDbContext dbcontext, IMapper mapper, IConfiguration configuration)
        {
            _dbcontext = dbcontext ?? throw new ArgumentNullException(nameof(dbcontext));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
        }

        public override HandlerResult<CompanyDto> HandleRequest(UpdateCompanyRequest request, CancellationToken cancellationToken, HandlerResult<CompanyDto> result)
        {
            var newCompanyDetails = request.CompanyDto;
            var modifiedBy = request.UserId;
            if(newCompanyDetails.Id != 0)
            {
                var companyDetails = _dbcontext.Companies.Where(p => p.Id == newCompanyDetails.Id).FirstOrDefault();
                var companyFolder = _dbcontext.Folders.Where(f => f.Id == companyDetails.RootFolderId).FirstOrDefault();
                
                companyDetails.CompanyName = newCompanyDetails.CompanyName;
                companyDetails.Status = newCompanyDetails.Status;
                companyFolder.Deleted = !newCompanyDetails.Status;
                
                _dbcontext.Companies.Update(companyDetails);
                _dbcontext.Folders.Update(companyFolder);
                _dbcontext.SaveChanges();

                if(request.CompanyDto.UpdateUsers)
                {
                    var allCompnayUsers = _dbcontext.AppUsers.Where(u => u.CompanyId == companyDetails.Id);
                    foreach(var user in allCompnayUsers)
                    {
                        user.Active = companyDetails.Status;
                    }
                    _dbcontext.AppUsers.UpdateRange(allCompnayUsers);
                    _dbcontext.SaveChanges();
                }
                result.Entity = _mapper.Map<CompanyDto>(_dbcontext.Companies.First(a => a.Id == newCompanyDetails.Id));
            }
            result.ResultType = ResultType.Success;
            return result;
        }

        public override Exception OnException(Exception ex)
        {
            return new Exception(ex.Message, ex);
        }
    }
}
