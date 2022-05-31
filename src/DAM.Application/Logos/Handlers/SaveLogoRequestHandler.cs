using AutoMapper;
using DAM.Application.Cache;
using DAM.Persistence;
using System;
using System.Linq;
using System.Threading;
using Microsoft.Extensions.Configuration;
using DAM.Application.Services.Interfaces;
using System.IO;
using DAM.Domain.Entities;
using DAM.Application.Logos.Dtos;
using DAM.Application.Logos.Requests;
using DAM.Application.AuditTrail.Enums;
using DAM.Application.Extensions;

namespace DAM.Application.Watermarks.Handlers
{
    public class SaveLogoRequestHandler : HandlerBase<SaveLogoRequest, HandlerResult<LogoDto>>
    {
        private readonly IDbContext _dbcontext;
        private readonly IMapper _mapper;
        private readonly IConfiguration _configuration;
        private readonly IAzureStorageService _azureStorageService;
        private readonly IHelperService _helperService;

        public SaveLogoRequestHandler(ICacheProvider cacheProvider, IDbContext dbcontext, IMapper mapper, IConfiguration configuration, IAzureStorageService azureStorageService, IHelperService helperService)
        {
            _dbcontext = dbcontext ?? throw new ArgumentNullException(nameof(dbcontext));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
            _azureStorageService = azureStorageService ?? throw new ArgumentNullException(nameof(azureStorageService));
            _helperService = helperService ?? throw new ArgumentNullException(nameof(helperService));
        }

        public override HandlerResult<LogoDto> HandleRequest(SaveLogoRequest request, CancellationToken cancellationToken, HandlerResult<LogoDto> result)
        {
            result.Entity = new LogoDto();
            var logo = new Logo();

            if (request.Logo.Id.HasValue)
            {
                logo = _dbcontext.Logos.FirstOrDefault(x => x.Id == request.Logo.Id.Value);

                if (logo == null)
                {
                    result.ResultType = ResultType.NoData;
                    return result;
                }
                else if(request.Logo.IsDeleted)
                {
                   if(request.Logo.IsApplied != true)
                    { logo.IsDeleted = true; }

                }
                else if (request.Logo.IsApplied)
                {
                    logo.IsApplied = (bool)request.Logo.IsApplied;
                }
                logo.ModifiedById = request.UserId;
                _dbcontext.Logos.Update(logo);
            }
            else
            {
                //save new logo
                logo = _mapper.Map<Logo>(request.Logo);

                if (request.Logo.Logo != null)
                {
                    // Save to blob
                    using (Stream wmStream = new MemoryStream(request.Logo.Logo))
                    {
                        _azureStorageService.SaveLogo(_configuration, wmStream, logo.FileName, out Uri blobUri);
                        logo.LogoUrl = _azureStorageService.GetLogoUrl(_configuration, logo.FileName);
                    }
                }
                logo.CreatedById = request.UserId;
                _dbcontext.Logos.Add(logo);
            }

          
            _dbcontext.SaveChanges();

            // Unapply other logos
            if (request.Logo.IsApplied && (bool)request.Logo.IsApplied)
            {
                var exLogos = _dbcontext.Logos.Where(x => x.Id != logo.Id);
                foreach (var exLogo in exLogos)
                {
                    exLogo.IsApplied = false;
                    logo.ModifiedById = request.UserId;
                }

                _dbcontext.Logos.UpdateRange(exLogos);
                _dbcontext.SaveChanges();
            }

            var auditTrailEntry = new AssetAudit()
            {
                AssetFileName = "Logo Change",
                AuditType = Convert.ToInt32(AssetAuditType.LogoChanged),
                AuditTypeText = AssetAuditType.LogoChanged.GetDescription(),
                AuditCreatedByUserId = request.UserId,
                AuditCreatedDate = DateTimeOffset.UtcNow,
                AuditCreatedByName = "",
                NewParameters = _helperService.GetJsonString(request.Logo),
            };

            _dbcontext.AssetAudit.Add(auditTrailEntry);
            _dbcontext.SaveChanges();

            result.Entity = _mapper.Map<LogoDto>(logo);
            result.ResultType = ResultType.Success;
            
            return result;
        }

        public override Exception OnException(Exception ex)
        {
            return new Exception(ex.Message, ex);
        }
    }
}