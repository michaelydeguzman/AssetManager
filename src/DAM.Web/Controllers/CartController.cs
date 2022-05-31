using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DAM.Application;
using DAM.Application.Carts.Dtos;
using DAM.Application.Carts.Requests;
using DAM.Application.Users.Models;
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
    public class CartController : ControllerBase
    {
        private IMediator _mediator;
        private readonly UserManager<ApplicationUser> _userManager;

        public CartController(IMediator mediator, UserManager<ApplicationUser> userManager)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
            _userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
        }

        [ProducesResponseType(typeof(HandlerResult<CartDto>), StatusCodes.Status200OK)]
        [Authorize(AuthenticationSchemes = JwtConfigConstants.AuthSchemes)]
        [HttpGet]
        [Route("Current")]
        public async Task<IActionResult> GetCurrentCart()
        {
            var userId = _userManager.GetUserId(User);
            
            var result = await _mediator.Send(new GetCurrentCartRequest(userId));

            if (result.ResultType == ResultType.BadRequest || result.ResultType == ResultType.Fail)
            {
                return BadRequest(new { result.Message });
            }

            return Ok(result.Entity);
        }

        [ProducesResponseType(typeof(HandlerResult<CartDto>), StatusCodes.Status200OK)]
        [Authorize(AuthenticationSchemes = JwtConfigConstants.AuthSchemes)]
        [DisableRequestSizeLimit]
        [HttpPut]
        [Route("Delete")]
        public async Task<IActionResult> DeleteCart(CartDto cartDto)
        {
            var result = await _mediator.Send(new DeleteCartRequest(cartDto));

            if (result.ResultType == ResultType.BadRequest || result.ResultType == ResultType.Fail)
            {
                return BadRequest(new { result.Message });
            }

            return Ok("OK");
        }


        [ProducesResponseType(typeof(HandlerResult<List<CartDto>>), StatusCodes.Status200OK)]
        [Authorize(AuthenticationSchemes = JwtConfigConstants.AuthSchemes)]
        [HttpGet]
        public async Task<IActionResult> GetCarts()
        {
            var userId = _userManager.GetUserId(User);

            var result = await _mediator.Send(new GetCartsRequest(userId));

            if (result.ResultType == ResultType.NoData)
            {
                return NotFound(new { result.Message });
            }
            if (result.ResultType == ResultType.BadRequest || result.ResultType == ResultType.Fail)
            {
                return BadRequest(new { result.Message });
            }

            return Ok(result.Entity);
        }

        [ProducesResponseType(typeof(HandlerResult<List<CartDto>>), StatusCodes.Status200OK)]
        [Authorize(AuthenticationSchemes = JwtConfigConstants.AuthSchemes)]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetCart(int id)
        {
            var result = await _mediator.Send(new GetCartRequest(id));

            if (result.ResultType == ResultType.NoData)
            {
                return NotFound(new { result.Message });
            }
            if (result.ResultType == ResultType.BadRequest || result.ResultType == ResultType.Fail)
            {
                return BadRequest(new { result.Message });
            }

            return Ok(result.Entity);
        }

        [ProducesResponseType(typeof(HandlerResult<CartDto>), StatusCodes.Status201Created)]
        [DisableRequestSizeLimit]
        [Authorize(AuthenticationSchemes = JwtConfigConstants.AuthSchemes)]
        [HttpPut]
        public async Task<IActionResult> AddCart(CartDto cartDto)
        {
            var userId = _userManager.GetUserId(User);

            var result = await _mediator.Send(new CreateCartRequest(cartDto, userId));

            if (result.ResultType == ResultType.NoData)
            {
                return NotFound(new { result.Message });
            }
            if (result.ResultType == ResultType.BadRequest || result.ResultType == ResultType.Fail)
            {
                return BadRequest(new { result.Message });
            }

            return Ok("OK");
        }
    }
}
