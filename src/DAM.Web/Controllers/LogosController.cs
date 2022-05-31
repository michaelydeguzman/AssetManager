using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DAM.Application;
using DAM.Application.Logos.Dtos;
using DAM.Application.Logos.Requests;
using DAM.Application.Styles.Dtos;
using DAM.Application.Styles.Requests;
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
    public class LogosController : ControllerBase
    {
        private IMediator _mediator;
        private readonly UserManager<ApplicationUser> _userManager;

        public LogosController(IMediator mediator, UserManager<ApplicationUser> userManager)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
            _userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
        }

        [ProducesResponseType(typeof(HandlerResult<IEnumerable<LogoDto>>), StatusCodes.Status200OK)]
        [HttpGet]
        public async Task<IActionResult> GetLogos()
        {
            var result = await _mediator.Send(new GetLogosRequest());

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
                Logos = result.Entity
            });
        }


        [ProducesResponseType(typeof(HandlerResult<LogoDto>), StatusCodes.Status200OK)]
        [Authorize(AuthenticationSchemes = JwtConfigConstants.AuthSchemes)]
        [HttpPost]
        [Route("Save")]
        public async Task<IActionResult> SaveLogo(LogoDto logo)
        {
            var userId = _userManager.GetUserId(User);
            var result = await _mediator.Send(new SaveLogoRequest(logo, userId));

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
                Logos = result.Entity,
            });

        }
    }
}
