using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DAM.Application;
using DAM.Application.Assets.Dtos;
using DAM.Application.Assets.Requests;
using DAM.Application.Folders.Dtos;
using DAM.Application.Users.Models;
using DAM.Domain.Entities.Identity;
using DAM.Persistence;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using static DAM.Application.Folders.Requests.FoldersRequest;

namespace DAM.Web.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LamController : ControllerBase
    {
        private IMediator _mediator;
        private readonly IDbContext _dbcontext;
        private readonly UserManager<ApplicationUser> _userManager;

        public LamController(IMediator mediator, UserManager<ApplicationUser> userManager, IDbContext dbcontext)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
            _userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
            _dbcontext = dbcontext ?? throw new ArgumentNullException(nameof(dbcontext));
        }

        [ProducesResponseType(typeof(HandlerResult<List<AssetDto>>), StatusCodes.Status200OK)]
        [HttpGet]
        [Route("Assets")]
        public async Task<IActionResult> GetAssets([FromQuery] string folders)
        {
            HandlerResult<IEnumerable<AssetVersionsDto>> result = new HandlerResult<IEnumerable<AssetVersionsDto>>();

            try
            {
                if (string.IsNullOrEmpty(folders))
                {
                    result = await _mediator.Send(new GetImageAssetsRequest(new List<int>()));
                }
                else
                {
                    var separated = folders.Split(new char[] { ',' });
                    List<int> parsed = separated.Select(s => int.Parse(s)).ToList();
                    result = await _mediator.Send(new GetImageAssetsRequest(parsed));
                }

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
                    Assets = result.Entity
                });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [ProducesResponseType(typeof(HandlerResult<List<AssetAuditDto>>), StatusCodes.Status200OK)]
        [HttpGet]
        [Route("Assets/Audit")]
        public async Task<IActionResult> GetAssetsAudit()
        {
            var result = await _mediator.Send(new GetAssetAuditRequest());

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
                Assets = result.Entity
            });
        }

        [ProducesResponseType(typeof(HandlerResult<List<FolderDto>>), StatusCodes.Status200OK)]
        [HttpGet]
        [Route("Folders")]
        public async Task<IActionResult> GetFolders()
        {
            var result = await _mediator.Send(new GetFolderRequest(0));

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
                Folders = result.Entity
            });
        }
    }
}