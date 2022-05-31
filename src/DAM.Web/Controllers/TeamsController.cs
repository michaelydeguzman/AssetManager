using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DAM.Application;
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
    public class TeamsController : ControllerBase
    {
        private IMediator _mediator;
        private readonly UserManager<ApplicationUser> _userManager;

        public TeamsController(IMediator mediator, UserManager<ApplicationUser> userManager)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
            _userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
        }

        [ProducesResponseType(typeof(HandlerResult<IEnumerable<TeamDto>>), StatusCodes.Status200OK)]
        [Authorize(AuthenticationSchemes = JwtConfigConstants.AuthSchemes)]
        [HttpGet]
        public async Task<IActionResult> GetTeams()
        {
            var result = await _mediator.Send(new TeamsRequest(0));

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
                Teams = result.Entity
            });
        }

        [ProducesResponseType(typeof(HandlerResult<IEnumerable<TeamDto>>), StatusCodes.Status200OK)]
        [Authorize(AuthenticationSchemes = JwtConfigConstants.AuthSchemes)]
        [HttpGet]
        [Route("{id}")]
        public async Task<IActionResult> GetTeamById(int id)
        {
            var result = await _mediator.Send(new TeamsRequest(id));

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
                Teams = result.Entity
            });
        }

        [ProducesResponseType(typeof(HandlerResult<TeamDto>), StatusCodes.Status200OK)]
        [Authorize(AuthenticationSchemes = JwtConfigConstants.AuthSchemes)]
        [HttpPost]
        [Route("Save")]
        public async Task<IActionResult> SaveTeam(TeamDto team)
        {
            var userId = _userManager.GetUserId(User);
            var result = await _mediator.Send(new SaveTeamRequest(team, userId));

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
                Team = result.Entity,
            });
        }

        [ProducesResponseType(typeof(HandlerResult<DeleteTeamsDto>), StatusCodes.Status201Created)]
        [Authorize(AuthenticationSchemes = JwtConfigConstants.AuthSchemes)]
        [HttpPost]
        [Route("Delete")]
        public async Task<IActionResult> DeleteProject(DeleteTeamsDto teams)
        {
            var userId = _userManager.GetUserId(User);

            var result = await _mediator.Send(new DeleteTeamsRequest(teams, userId));

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
                DeleteTeamsDto = result.Entity
            });
        }
    }
}
