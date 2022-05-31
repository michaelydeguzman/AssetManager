using System;
using DAM.Application.Assets.Dtos;
using DAM.Application.Assets.Requests;
using DAM.Application.Cache;
using DAM.Persistence;
using AutoMapper;
using Microsoft.Extensions.Configuration;
using System.Threading;
using System.Linq;
using DAM.Application.Services.Interfaces;
using DAM.Application.Emails.Dtos;

namespace DAM.Application.Assets.Handlers
{
    public class ShareAssetRequestHandler : HandlerBase<ShareAssetRequest, HandlerResult<ShareDto>>
    {
        private readonly ICacheProvider _cacheProvider;
        private readonly IDbContext _dbcontext;
        private readonly IMapper _mapper;
        private readonly IConfiguration _configuration;
        private readonly IEmailService _emailService;

        public ShareAssetRequestHandler(ICacheProvider cacheProvider, IDbContext dbcontext, IMapper mapper, IConfiguration configuration, IEmailService emailService)
        {
            _cacheProvider = cacheProvider ?? throw new ArgumentNullException(nameof(cacheProvider));
            _dbcontext = dbcontext ?? throw new ArgumentNullException(nameof(dbcontext));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
            _emailService = emailService ?? throw new ArgumentNullException(nameof(emailService));
        }

        public override HandlerResult<ShareDto> HandleRequest(ShareAssetRequest request, CancellationToken cancellationToken, HandlerResult<ShareDto> result)
        {
            var asset =_mapper.Map<AssetDto>(_dbcontext.AssetVersions.First(a => a.Key == request.ShareDto.AssetKey));
            var emailTemplate = _mapper.Map<EmailTemplateDto>(_dbcontext.EmailTemplates.First(a => a.EmailTemplateKey == _configuration["ShareTemplate"]));

            var uri = _configuration["BaseUrl"] + "api/Assets/Download/" + request.ShareDto.AssetKey + "/" + request.ShareDto.EmailAddress;

            var body = emailTemplate.Contents.Replace("%%DownloadUrl%%", uri);

            _emailService.SendEmail(_configuration, emailTemplate.Subject, request.ShareDto.EmailAddress, body);
            result.ResultType = ResultType.Success;
            return result;
        }

        public override Exception OnException(Exception ex)
        {
            return new Exception(ex.Message, ex);
        }
    }
}
