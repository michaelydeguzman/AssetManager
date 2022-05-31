using AutoMapper;
using DAM.Application.Assets.Dtos;
using DAM.Application.Cache;
using DAM.Application.Services.Interfaces;
using DAM.Application.Users.Dtos;
using DAM.Domain.Entities;
using DAM.Domain.Entities.Identity;
using DAM.Persistence;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using static DAM.Application.Users.Requests.UserRequest;

namespace DAM.Application.Users.Handlers
{
    public class InviteNewUserRequestHandler : HandlerBase<InviteNewUserRequest, HandlerResult<InviteNewUserDto>>
    {
        private readonly ICacheProvider _cacheProvider;
        private readonly IDbContext _dbcontext;
        private readonly IMapper _mapper;
        private readonly IConfiguration _configuration;
        private readonly IHelperService _helperService;
        private readonly IEmailService _emailService;

        public InviteNewUserRequestHandler(ICacheProvider cacheProvider, IDbContext dbcontext, IMapper mapper, IConfiguration configuration, IHelperService helperService, IEmailService emailService)
        {
            _cacheProvider = cacheProvider ?? throw new ArgumentNullException(nameof(cacheProvider));
            _dbcontext = dbcontext ?? throw new ArgumentNullException(nameof(dbcontext));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
            _helperService = helperService ?? throw new ArgumentNullException(nameof(helperService));
            _emailService = emailService ?? throw new ArgumentNullException(nameof(emailService));
        }
        public override HandlerResult<InviteNewUserDto> HandleRequest(InviteNewUserRequest request, CancellationToken cancellationToken, HandlerResult<InviteNewUserDto> result)
        {
            //var userDetails = request.InviteNewUserDto;
            //var existingUser = _dbcontext.Users.Where(u => u.EmailAddress == userDetails.EmailAddress && u.Active).FirstOrDefault();
            //if(existingUser == null)
            //{
            //    var user = _mapper.Map<ApplicationUser>(userDetails);
            //    user.Active = false;
            //    user.Password = _helperService.EncodeUsingMD5("Simple5%");

            //    _dbcontext.Users.Add(user);
            //    _dbcontext.SaveChanges();

            //    var partnerRootFolder = userDetails.RootFolderId;
            //    if (userDetails.PartnerId != 0) //if partner access only
            //    {
            //        partnerRootFolder = _dbcontext.Partners.Where(p => p.Id == userDetails.PartnerId).FirstOrDefault().RootFolderId;
            //        _dbcontext.UserPartners.Add(new UserPartner()
            //        {
            //            UserId = user.Id,
            //            PartnerId = userDetails.PartnerId
            //        });
            //        _dbcontext.SaveChanges();
            //    }

            //    _dbcontext.UserFolders.Add(new UserFolder()
            //    {
            //        UserId = user.Id,
            //        FolderId = partnerRootFolder
            //    });
            //    _dbcontext.SaveChanges();

            //    var emailTemplate = _mapper.Map<EmailTemplateDto>(_dbcontext.EmailTemplates.First(a => a.EmailTemplateKey == _configuration["InviteUserTemplate"]));
            //    var body = emailTemplate.Contents
            //        .Replace("%%Username%%", userDetails.EmailAddress)
            //        .Replace("%%BaseUrl%%", _configuration["BaseUrl"]);

            //    _emailService.SendEmail(_configuration, emailTemplate.Subject, userDetails.EmailAddress, body);
            //}
            return result;
        }
    }
}
