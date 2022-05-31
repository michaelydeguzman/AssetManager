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
    public class AddCompanyRequestHandler : HandlerBase<AddCompanyRequest, HandlerResult<CompanyDto>>
    {
        private readonly IDbContext _dbcontext;
        private readonly IMapper _mapper;
        private readonly IConfiguration _configuration;

        public AddCompanyRequestHandler(IDbContext dbcontext, IMapper mapper, IConfiguration configuration)
        {
            _dbcontext = dbcontext ?? throw new ArgumentNullException(nameof(dbcontext));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
        }

        public override HandlerResult<CompanyDto> HandleRequest(AddCompanyRequest request, CancellationToken cancellationToken, HandlerResult<CompanyDto> result)
        {
            var createdBy = request.UserId;
            var partnerDto = request.PartnerDto;
            var existingPartner = _dbcontext.Companies.Where(p => p.CompanyName == request.PartnerDto.CompanyName).FirstOrDefault();
            
            if (existingPartner == null)
            {
                var partnerFolder = _dbcontext.Folders.FirstOrDefault(f => f.Id == 2);

                var company = new Company();
                var folder = new Folder();
                // var battleCards = new Folder
                // {
                //     FolderName = "Battle Cards",
                //     Description = "Battle Cards",
                //     Deleted = false
                // };
                // var marketing = new Folder
                // {
                //     FolderName = "Marketing",
                //     Description = "Marketing",
                //     Deleted = false
                // };
                // var presentations = new Folder
                // {
                //     FolderName = "Presentations",
                //     Description = "Presentations",
                //     Deleted = false
                // };
                // var whitePapers = new Folder
                // {
                //     FolderName = "White Papers",
                //     Description = "White Papers",
                //     Deleted = false
                // };

                //add folder after adding partner
                folder.FolderName = partnerDto.CompanyName;
                folder.Description = partnerDto.CompanyName;
                folder.ParentFolderId = partnerFolder.Id;
                folder.Deleted = false;

                _dbcontext.Folders.Add(folder);
                _dbcontext.SaveChanges();

                //add default sub folders
                // battleCards.ParentFolderId = folder.Id;
                // _dbcontext.Folders.Add(battleCards);
                // marketing.ParentFolderId = folder.Id;
                // _dbcontext.Folders.Add(marketing);
                // presentations.ParentFolderId = folder.Id;
                // _dbcontext.Folders.Add(presentations);
                // whitePapers.ParentFolderId = folder.Id;
                // _dbcontext.Folders.Add(whitePapers);

                partnerDto.RootFolderId = folder.Id;
                partnerDto.CreatedById = createdBy;
                company = _mapper.Map<Company>(partnerDto);
                _dbcontext.Companies.Add(company);
                _dbcontext.SaveChanges();
                result.Entity = _mapper.Map<CompanyDto>(_dbcontext.Companies.First(a => a.Id == company.Id));
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
