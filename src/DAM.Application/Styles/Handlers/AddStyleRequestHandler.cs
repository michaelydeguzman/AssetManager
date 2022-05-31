using AutoMapper;
using DAM.Application.Cache;
using DAM.Persistence;
using System;
using System.Linq;
using System.Threading;
using Microsoft.Extensions.Configuration;
using DAM.Application.Services.Interfaces;
using DAM.Domain.Entities;
using DAM.Application.Styles.Dtos;
using DAM.Application.AuditTrail.Enums;
using DAM.Application.Extensions;
using static DAM.Application.Styles.Requests.StyleRequests;
using System.IO;

namespace DAM.Application.Styles.Handlers
{
    public class AddStyleRequestHandler : HandlerBase<AddStyleRequest, HandlerResult<ThemeDto>>
    {
        private readonly IDbContext _dbcontext;
        private readonly IMapper _mapper;
        private readonly IConfiguration _configuration;
        private readonly IHelperService _helperService;
        private readonly IAzureStorageService _azureStorageService;

        public AddStyleRequestHandler(ICacheProvider cacheProvider, IDbContext dbcontext, IMapper mapper, IConfiguration configuration, IHelperService helperService, IAzureStorageService azureStorageService)
        {
            _dbcontext = dbcontext ?? throw new ArgumentNullException(nameof(dbcontext));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
            _helperService = helperService ?? throw new ArgumentNullException(nameof(helperService));
            _azureStorageService = azureStorageService ?? throw new ArgumentNullException(nameof(azureStorageService));
        }

        public override HandlerResult<ThemeDto> HandleRequest(AddStyleRequest request, CancellationToken cancellationToken, HandlerResult<ThemeDto> result)
        {
            result.Entity = new ThemeDto();
            var style = new Theme();
            
            //save new style
            style = _mapper.Map<Theme>(request.Style);
            style.Deleted = false;
            style.IsApplied = (bool)request.Style.IsApplied;
            style.CreatedById = request.UserId;
            style.LogoFileName = request.Style.Name;
            style.LogoKey = Guid.NewGuid().ToString().Replace("-", "");
            if (request.Style.Logo != null)
            {
                // Save to blob
                using (Stream wmStream = new MemoryStream(request.Style.Logo))
                {
                    _azureStorageService.SaveLogo(_configuration, wmStream, style.LogoKey, out Uri blobUri);
                    style.LogoUrl = _azureStorageService.GetLogoUrl(_configuration, style.LogoKey);
                }
            }
            _dbcontext.Themes.Add(style);
            _dbcontext.SaveChanges();

            // Unapply other styles
            if(request.Style.IsApplied.HasValue && (bool)request.Style.IsApplied)
            {
                var unApplyStyles = _dbcontext.Themes.Where(x => x.Id != style.Id);
                foreach (var exStyle in unApplyStyles)
                {
                    exStyle.IsApplied = false;
                    exStyle.ModifiedById = request.UserId;
                }

                _dbcontext.Themes.UpdateRange(unApplyStyles);
                _dbcontext.SaveChanges();
            }

            var defaultStyle = _dbcontext.Themes.FirstOrDefault(x => x.IsApplied); 
            if(defaultStyle == null)
            {
                defaultStyle = _dbcontext.Themes.FirstOrDefault();
                defaultStyle.IsApplied = true;
                defaultStyle.ModifiedById = request.UserId;
                _dbcontext.Themes.Update(defaultStyle);
                _dbcontext.SaveChanges();
            }

            var auditTrailEntry = new AssetAudit()
            {
                AssetFileName = "Theme Add",
                AuditType = Convert.ToInt32(AssetAuditType.ThemeAdded),
                AuditTypeText = AssetAuditType.ThemeAdded.GetDescription(),
                AuditCreatedByUserId = request.UserId,
                AuditCreatedDate = DateTimeOffset.UtcNow,
                AuditCreatedByName = "",
                NewParameters = _helperService.GetJsonString(request.Style),
            };

            _dbcontext.AssetAudit.Add(auditTrailEntry);
            _dbcontext.SaveChanges();

            result.Entity = _mapper.Map<ThemeDto>(style);
            result.ResultType = ResultType.Success;

            return result;
        }

        public override Exception OnException(Exception ex)
        {
            return new Exception(ex.Message, ex);
        }
    }
}