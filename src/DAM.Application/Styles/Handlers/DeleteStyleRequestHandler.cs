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
using System.Collections.Generic;

namespace DAM.Application.Styles.Handlers
{
    public class DeleteStyleRequestHandler : HandlerBase<DeleteStyleRequest, HandlerResult<List<ThemeDto>>>
    {
        private readonly IDbContext _dbcontext;
        private readonly IMapper _mapper;
        private readonly IConfiguration _configuration;
        private readonly IHelperService _helperService;

        public DeleteStyleRequestHandler(ICacheProvider cacheProvider, IDbContext dbcontext, IMapper mapper, IConfiguration configuration, IHelperService helperService)
        {
            _dbcontext = dbcontext ?? throw new ArgumentNullException(nameof(dbcontext));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
            _helperService = helperService ?? throw new ArgumentNullException(nameof(helperService));
        }

        public override HandlerResult<List<ThemeDto>> HandleRequest(DeleteStyleRequest request, CancellationToken cancellationToken, HandlerResult<List<ThemeDto>> result)
        {
            result.Entity = new List<ThemeDto>();
            //var style = new Style();

            foreach (var style in request.Styles)
            {
                var style_entity = _dbcontext.Themes.FirstOrDefault(x => x.Id == style.Id.Value);

                if (style_entity == null)
                {
                    result.ResultType = ResultType.NoData;
                    return result;
                }
                if(style_entity.IsApplied == false)
                {
                    style_entity.Deleted = true;
                    style_entity.ModifiedById = request.UserId;
                    style_entity.ModifiedDate = DateTimeOffset.UtcNow;
                    _dbcontext.Themes.Update(style_entity);
                    _dbcontext.SaveChanges();
                }
            }
            var auditTrailEntry = new AssetAudit()
            {
                AssetFileName = "Theme Change",
                AuditType = Convert.ToInt32(AssetAuditType.ThemeChanged),
                AuditTypeText = AssetAuditType.ThemeChanged.GetDescription(),
                AuditCreatedByUserId = request.UserId,
                AuditCreatedDate = DateTimeOffset.UtcNow,
                AuditCreatedByName = "",
                NewParameters = _helperService.GetJsonString(request.Styles),
            };

            _dbcontext.AssetAudit.Add(auditTrailEntry);
            _dbcontext.SaveChanges();

            result.ResultType = ResultType.Success;

            return result;
        }

        public override Exception OnException(Exception ex)
        {
            return new Exception(ex.Message, ex);
        }
    }
}