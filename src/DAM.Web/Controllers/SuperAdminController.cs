using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DAM.Application;
using DAM.Application.Accounts.Dtos;
using DAM.Application.Accounts.Requests;
using DAM.Application.Users.Models;
using DAM.Domain.Entities.Identity;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace DAM.Web.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SuperAdminController : ControllerBase
    {
        private IMediator _mediator;
        private readonly UserManager<ApplicationUser> _userManager;
        private IConfiguration _configuration;

        public SuperAdminController(IMediator mediator, UserManager<ApplicationUser> userManager, IConfiguration configuration)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
            _userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));

        }

        [ProducesResponseType(typeof(HandlerResult<List<AccountDto>>), StatusCodes.Status200OK)]
        [Authorize(AuthenticationSchemes = JwtConfigConstants.AuthSchemes)]
        [HttpGet]
        [Route("ConfirmEmail/{id}")]
        public async Task<IActionResult> ConfirmEmail(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            
            var userId = _userManager.GetUserId(User);
            var currentUser = await _userManager.FindByIdAsync(userId);
            
            if (currentUser.Email != _configuration["DefaultUser"])
            {
                return BadRequest(
                    new
                    {
                        Message = "Not Super Admin!"
                    });
            }
            
            if (user == null)
            {
                return NotFound(
                    new
                    {
                        Message = AppMessageConstants.EmailConfirmationFailed + AppMessageConstants.InvalidEmail
                    });
            }

            var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            var result = await _userManager.ConfirmEmailAsync(user, code);

            if (result.Succeeded)
            {
                return Ok(new
                {
                    Message = "Confirmed successfully!"
                });
            }
            else
            {
                return BadRequest(new
                {
                    Message = AppMessageConstants.EmailConfirmationFailed + result.Errors.First().Description
                });
            }

            
        }

        [ProducesResponseType(typeof(HandlerResult<List<AccountDto>>), StatusCodes.Status200OK)]
        [Authorize(AuthenticationSchemes = JwtConfigConstants.AuthSchemes)]
        [HttpPost]
        [Route("ResetPassword")]
        public async Task<IActionResult> ResetPassword(ResetPasswordDto resetPasswordDetails)
        {
            var userId = _userManager.GetUserId(User);
            var currentUser = await _userManager.FindByIdAsync(userId);
            
            if (currentUser.Email != _configuration["DefaultUser"])
            {
                return BadRequest(
                    new
                    {
                        Message = "Not Super Admin!"
                    });
            }

            var user = await _userManager.FindByEmailAsync(resetPasswordDetails.Email);
            var token = await _userManager.GeneratePasswordResetTokenAsync(user);

            if (user == null)
            {
                return NotFound(new
                {
                    Message = AppMessageConstants.ResetPasswordFailed + " " + AppMessageConstants.InvalidEmail
                });
            }

            var result = await _userManager.ResetPasswordAsync(user, token, resetPasswordDetails.Password);

            if (result.Succeeded)
            {
                return Ok(new
                {
                    Message = "Reset Done!"
                });
            }
            else
            {
                return BadRequest(new
                {
                    Message = AppMessageConstants.ResetPasswordFailed + " " + result.Errors.First().Description
                });
            }
        }

        [ProducesResponseType(typeof(HandlerResult<List<AccountDto>>), StatusCodes.Status200OK)]
        [Authorize(AuthenticationSchemes = JwtConfigConstants.AuthSchemes)]
        [HttpGet]
        public async Task<IActionResult> AddUser()
        {
            //var result = await _mediator.Send(new GetAccountsRequest());

            return Ok();
        }

        [ProducesResponseType(typeof(HandlerResult<List<AccountDto>>), StatusCodes.Status200OK)]
        [Authorize(AuthenticationSchemes = JwtConfigConstants.AuthSchemes)]
        [HttpGet]
        public async Task<IActionResult> DeleteUser()
        {
            //var result = await _mediator.Send(new GetAccountsRequest());

            return Ok();
        }

    }
}