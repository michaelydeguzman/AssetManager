using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DAM.Application;
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
using static DAM.Application.Styles.Requests.StyleRequests;

namespace DAM.Web.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StylesController : ControllerBase
    {
        private IMediator _mediator;
        private readonly UserManager<ApplicationUser> _userManager;

        public StylesController(IMediator mediator, UserManager<ApplicationUser> userManager)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
            _userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
        }

        [ProducesResponseType(typeof(HandlerResult<IEnumerable<ThemeDto>>), StatusCodes.Status200OK)]
        [HttpGet]
        public async Task<IActionResult> GetStyles()
        {
            var result = await _mediator.Send(new GetStylesRequest());

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
                Styles = result.Entity
            });
        }

        [ProducesResponseType(typeof(HandlerResult<ThemeDto>), StatusCodes.Status200OK)]
        [Authorize(AuthenticationSchemes = JwtConfigConstants.AuthSchemes)]
        [HttpPost]
        [Route("Save")]
        public async Task<IActionResult> UpdateStyle(ThemeDto style)
        {
            var userId = _userManager.GetUserId(User);
            var result = await _mediator.Send(new EditStyleRequest(style, userId));

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
                Styles = result.Entity,
            });
        }

        [ProducesResponseType(typeof(HandlerResult<ThemeDto>), StatusCodes.Status200OK)]
        [Authorize(AuthenticationSchemes = JwtConfigConstants.AuthSchemes)]
        [HttpPost]
        [Route("Add")]
        public async Task<IActionResult> AddStyle(ThemeDto style)
        {
            var userId = _userManager.GetUserId(User);
            var result = await _mediator.Send(new AddStyleRequest(style, userId));

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
                Styles = result.Entity,
            });
        }

        [ProducesResponseType(typeof(HandlerResult<ThemeDto>), StatusCodes.Status200OK)]
        [Authorize(AuthenticationSchemes = JwtConfigConstants.AuthSchemes)]
        [HttpPost]
        [Route("Delete")]
        public async Task<IActionResult> DeleteStyle(List<ThemeDto> styles)
        {
            var userId = _userManager.GetUserId(User);
            var result = await _mediator.Send(new DeleteStyleRequest(styles, userId));

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
                Styles = result.Entity,
            });
        }

        [ProducesResponseType(typeof(HandlerResult<ThemeDto>), StatusCodes.Status200OK)]
        [Authorize(AuthenticationSchemes = JwtConfigConstants.AuthSchemes)]
        [HttpPost]
        [Route("Copy")]
        public async Task<IActionResult> CopyStyle(ThemeDto style)
        {
            var userId = _userManager.GetUserId(User);
            var result = await _mediator.Send(new CopyStyleRequest(style, userId));

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
                Styles = result.Entity,
            });
        }
    }
}
