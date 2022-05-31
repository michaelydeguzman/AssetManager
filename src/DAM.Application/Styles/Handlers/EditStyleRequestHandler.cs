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
using DAM.Application.Styles.Dtos;
using DAM.Application.Styles.Requests;
using DAM.Application.AuditTrail.Enums;
using DAM.Application.Extensions;
using static DAM.Application.Styles.Requests.StyleRequests;

namespace DAM.Application.Styles.Handlers
{
    public class EditStyleRequestHandler : HandlerBase<EditStyleRequest, HandlerResult<ThemeDto>>
    {
        private readonly IDbContext _dbcontext;
        private readonly IMapper _mapper;
        private readonly IConfiguration _configuration;
        private readonly IAzureStorageService _azureStorageService;
        private readonly IHelperService _helperService;

        public EditStyleRequestHandler(ICacheProvider cacheProvider, IDbContext dbcontext, IMapper mapper, IConfiguration configuration, IAzureStorageService azureStorageService, IHelperService helperService)
        {
            _dbcontext = dbcontext ?? throw new ArgumentNullException(nameof(dbcontext));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
            _azureStorageService = azureStorageService ?? throw new ArgumentNullException(nameof(azureStorageService));
            _helperService = helperService ?? throw new ArgumentNullException(nameof(helperService));
        }

        public override HandlerResult<ThemeDto> HandleRequest(EditStyleRequest request, CancellationToken cancellationToken, HandlerResult<ThemeDto> result)
        {
            result.Entity = new ThemeDto();
            var style = new Theme();

            if (request.Style.Id.HasValue)
            {
                style = _dbcontext.Themes.FirstOrDefault(x => x.Id == request.Style.Id.Value);

                if (style == null)
                {
                    result.ResultType = ResultType.NoData;
                    return result;
                }
                else if (request.Style.IsApplied.HasValue)
                {
                    style.IsApplied = (bool)request.Style.IsApplied;
                    style.ModifiedById = request.UserId;
                    style.ModifiedDate = DateTimeOffset.UtcNow;
                    _dbcontext.Themes.Update(style);
                }
                else if (request.Style.Id > 2 && !request.Style.IsApplied.HasValue)
                {
                    style.Name = request.Style.Name;
                    style.PrimaryColor = request.Style.PrimaryColor;
                    style.SecondaryColor = request.Style.SecondaryColor;
                    style.TertiaryColor = request.Style.TertiaryColor;
                    style.ModifiedById = request.UserId;
                    style.ModifiedDate = DateTimeOffset.UtcNow;
                    if (request.Style.Logo != null)
                    {
                        // Save to blob
                        using (Stream wmStream = new MemoryStream(request.Style.Logo))
                        {
                            _azureStorageService.SaveLogo(_configuration, wmStream, style.LogoKey, out Uri blobUri);
                            style.LogoUrl = _azureStorageService.GetLogoUrl(_configuration, style.LogoKey);
                        }
                    }
                    _dbcontext.Themes.Update(style);
                }
                _dbcontext.SaveChanges();
            }

            // Unapply other styles
            if (request.Style.IsApplied.HasValue && (bool)request.Style.IsApplied)
            {
                var unApplyStyles = _dbcontext.Themes.Where(x => x.Id != request.Style.Id);
                foreach (var exStyle in unApplyStyles)
                {
                    exStyle.IsApplied = false;
                    exStyle.ModifiedById = request.UserId;
                    exStyle.ModifiedDate = DateTimeOffset.UtcNow;
                }

                _dbcontext.Themes.UpdateRange(unApplyStyles);
                _dbcontext.SaveChanges();
            }

            var defaultStyle = _dbcontext.Themes.FirstOrDefault(x => x.IsApplied);
            if (defaultStyle == null)
            {
                defaultStyle = _dbcontext.Themes.FirstOrDefault();
                defaultStyle.IsApplied = true;
                defaultStyle.ModifiedById = request.UserId;
                defaultStyle.ModifiedDate = DateTimeOffset.UtcNow;
                _dbcontext.Themes.Update(defaultStyle);
                _dbcontext.SaveChanges();
            }

            var auditTrailEntry = new AssetAudit()
            {
                AssetFileName = "Theme Change",
                AuditType = Convert.ToInt32(AssetAuditType.ThemeChanged),
                AuditTypeText = AssetAuditType.ThemeChanged.GetDescription(),
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