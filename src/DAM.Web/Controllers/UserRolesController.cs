using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DAM.Application;
using DAM.Application.UserRoles.Dtos;
using DAM.Application.Users.Models;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using static DAM.Application.UserRoles.Requests.UserRolesRequest;

namespace DAM.Web.Controllers
{
    [Route("/api/[controller]")]
    [ApiController]
    public class UserRolesController : ControllerBase
    {
        private IMediator _mediator;

        public UserRolesController(IMediator mediator)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        [ProducesResponseType(typeof(HandlerResult<List<UserRoleDto>>), StatusCodes.Status200OK)]
        [Authorize(AuthenticationSchemes = JwtConfigConstants.AuthSchemes)]
        [HttpGet]
        public async Task<IActionResult> GetAllUserRoles()
        {
            var result = await _mediator.Send(new GetAllUserRolesRequest());

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
                UserRoles = result.Entity,
            });
        }
    }
}
