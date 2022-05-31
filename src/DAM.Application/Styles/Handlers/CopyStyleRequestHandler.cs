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

namespace DAM.Application.Styles.Handlers
{
    public class CopyStyleRequestHandler : HandlerBase<CopyStyleRequest, HandlerResult<ThemeDto>>
    {
        private readonly IDbContext _dbcontext;
        private readonly IMapper _mapper;
        private readonly IConfiguration _configuration;
        private readonly IHelperService _helperService;

        public CopyStyleRequestHandler(ICacheProvider cacheProvider, IDbContext dbcontext, IMapper mapper, IConfiguration configuration, IHelperService helperService)
        {
            _dbcontext = dbcontext ?? throw new ArgumentNullException(nameof(dbcontext));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
            _helperService = helperService ?? throw new ArgumentNullException(nameof(helperService));
        }

        public override HandlerResult<ThemeDto> HandleRequest(CopyStyleRequest request, CancellationToken cancellationToken, HandlerResult<ThemeDto> result)
        {
            result.Entity = new ThemeDto();

            //copy new style
            var style_entity = new Theme();
            var style_copy = _dbcontext.Themes.FirstOrDefault(x => x.Id == request.Style.Id.Value);
            style_entity.Deleted = false;
            style_entity.IsApplied = false;
            style_entity.CreatedById = request.UserId;
            style_entity.LogoFileName = style_copy.Name;
            style_entity.LogoKey = Guid.NewGuid().ToString().Replace("-", "");
            style_entity.LogoUrl = style_copy.LogoUrl;
            style_entity.PrimaryColor = style_copy.PrimaryColor;
            style_entity.SecondaryColor = style_copy.SecondaryColor;
            style_entity.TertiaryColor = style_copy.TertiaryColor;
            style_entity.Name = style_copy.Name;
            _dbcontext.Themes.Add(style_entity);
            _dbcontext.SaveChanges();

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

            result.Entity = _mapper.Map<ThemeDto>(style_entity);
            result.ResultType = ResultType.Success;

            return result;
        }

        public override Exception OnException(Exception ex)
        {
            return new Exception(ex.Message, ex);
        }
    }
}