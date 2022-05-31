using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DAM.Application;
using DAM.Application.Companies.Dtos;
using DAM.Application.Users.Models;
using DAM.Domain.Entities.Identity;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using static DAM.Application.Companies.Requests.CompanyRequests;

namespace DAM.Web.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CompanyController : Controller
    {
        private IMediator _mediator;
        private readonly UserManager<ApplicationUser> _userManager;

        public CompanyController(IMediator mediator, UserManager<ApplicationUser> userManager)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
            _userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
        }

        [ProducesResponseType(typeof(HandlerResult<List<CompanyDto>>), StatusCodes.Status200OK)]
        [Authorize(AuthenticationSchemes = JwtConfigConstants.AuthSchemes)]
        [HttpGet]
        public async Task<IActionResult> GetCompanies()
        {
            var result = await _mediator.Send(new GetCompaniesRequest());

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
                Companies = result.Entity,
            });
        }

        [ProducesResponseType(typeof(HandlerResult<List<CompanyDto>>), StatusCodes.Status200OK)]
        [Authorize(AuthenticationSchemes = JwtConfigConstants.AuthSchemes)]
        [HttpGet]
        [Route("{id}")]
        public async Task<IActionResult> GetCompany(int id)
        {
            var result = await _mediator.Send(new GetCompanyRequest(id));

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
                Companies = result.Entity,
            });
        }

        [ProducesResponseType(typeof(HandlerResult<CompanyDto>), StatusCodes.Status201Created)]
        [Authorize(AuthenticationSchemes = JwtConfigConstants.AuthSchemes)]
        [HttpPost]
        [Route("AddCompany")]
        public async Task<IActionResult> AddCompany(CompanyDto partner)
        {
            var userId = _userManager.GetUserId(User);
            var result = await _mediator.Send(new AddCompanyRequest(partner, userId));

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
                Companies = result.Entity,
            });
        }

        [ProducesResponseType(typeof(HandlerResult<CompanyDto>), StatusCodes.Status201Created)]
        [Authorize(AuthenticationSchemes = JwtConfigConstants.AuthSchemes)]
        [HttpPut]
        [Route("UpdateCompany")]
        public async Task<IActionResult> UpdateCompany(CompanyDto partner)
        {
            var userId = _userManager.GetUserId(User);
            var result = await _mediator.Send(new UpdateCompanyRequest(partner, userId));

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
                Companies = result.Entity,
            });
        }
    }
}
