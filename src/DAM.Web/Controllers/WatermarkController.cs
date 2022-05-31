using System;
using System.Threading.Tasks;
using DAM.Application;
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
    public class WatermarkController : ControllerBase
    {
        private IMediator _mediator;
        private readonly UserManager<ApplicationUser> _userManager;

        public WatermarkController(IMediator mediator, UserManager<ApplicationUser> userManager)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
            _userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
        }

        [ProducesResponseType(typeof(HandlerResult<WatermarkDto>), StatusCodes.Status200OK)]
        [Authorize(AuthenticationSchemes = JwtConfigConstants.AuthSchemes)]
        [HttpGet]
        [Route("Default")]
        public async Task<IActionResult> GetDefaultWatermark()
        {
            var result = await _mediator.Send(new GetDefaultWatermarkRequest());

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
                Watermark = result.Entity
            });
        }

        [ProducesResponseType(typeof(HandlerResult<WatermarkDto>), StatusCodes.Status200OK)]
        [Authorize(AuthenticationSchemes = JwtConfigConstants.AuthSchemes)]
        [HttpPost]
        [Route("Default/Save")]
        public async Task<IActionResult> UpdateFolder(WatermarkDto watermark)
        {
            var userId = _userManager.GetUserId(User);
            var result = await _mediator.Send(new SaveDefaultWatermarkRequest(watermark, userId));

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
                Watermark = result.Entity,
            });

        }
    }
}
