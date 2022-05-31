using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DAM.Application;
using DAM.Application.Emails.Dtos;
using DAM.Application.Emails.Requests;
using DAM.Application.Styles.Dtos;
using DAM.Application.Styles.Requests;
using DAM.Application.Teams.Dtos;
using DAM.Application.Teams.Requests;
using DAM.Application.Users.Models;
using DAM.Application.Watermarks.Dto;
using DAM.Application.Watermarks.Requests;
using DAM.Domain.Entities.Identity;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace DAM.Web.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EmailTemplatesController : ControllerBase
    {
        private IMediator _mediator;
        private readonly UserManager<ApplicationUser> _userManager;

        public EmailTemplatesController(IMediator mediator, UserManager<ApplicationUser> userManager)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
            _userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
        }

        [ProducesResponseType(typeof(HandlerResult<IEnumerable<EmailTemplateDto>>), StatusCodes.Status200OK)]
        [Authorize(AuthenticationSchemes = JwtConfigConstants.AuthSchemes)]
        [HttpGet]
        public async Task<IActionResult> GetEmailTemplates()
        {
            var result = await _mediator.Send(new EmailTemplatesRequest(0));

            if (result.ResultType == ResultType.NoData)
            {
                return NotFound(new { result.Message });
            }
            if (result.ResultType == ResultType.BadRequest || result.ResultType == ResultType.Fail)
            {
                return BadRequest(new { result.Message });
            }
            return Ok(new
            {
                EmailTemplates = result.Entity
            });
        }

        [ProducesResponseType(typeof(HandlerResult<EmailTemplateDto>), StatusCodes.Status200OK)]
        [Authorize(AuthenticationSchemes = JwtConfigConstants.AuthSchemes)]
        [HttpPost]
        [Route("Save")]
        public async Task<IActionResult> SaveEmailTemplate(EmailTemplateDto emailTemplate)
        {
            var userId = _userManager.GetUserId(User);
            var result = await _mediator.Send(new SaveEmailTemplateRequest(emailTemplate, userId));

            if (result.ResultType == ResultType.NoData)
            {
                return NotFound(new { result.Message });
            }
            if (result.ResultType == ResultType.BadRequest || result.ResultType == ResultType.Fail)
            {
                return BadRequest(new { result.Message });
            }

            return Ok(new
            {
                EmailTemplate = result.Entity,
            });
        }
    }
}
