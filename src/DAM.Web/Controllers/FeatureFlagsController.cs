using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DAM.Application;
using DAM.Application.Accounts.Dtos;
using DAM.Application.Accounts.Requests;
using DAM.Application.FeatureFlags.Dtos;
using DAM.Application.FeatureFlags.Requests;
using DAM.Application.Templates;
using DAM.Application.Users.Dtos;
using DAM.Application.Users.Exceptions;
using DAM.Application.Users.Models;
using DAM.Application.Users.Requests;
using DAM.Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace DAM.Web.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FeatureFlagsController : ControllerBase
    {
        private IMediator _mediator;

        public FeatureFlagsController(IMediator mediator)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        [ProducesResponseType(typeof(HandlerResult<List<FeatureFlagDto>>), StatusCodes.Status200OK)]
        //[Authorize(AuthenticationSchemes = JwtConfigConstants.AuthSchemes)]
        [HttpGet]
        public async Task<IActionResult> GetFeatureFlags()
        {
            var result = await _mediator.Send(new GetFeatureFlagsRequest());

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
                FeatureFlags = result.Entity
            });
        }

    }
}