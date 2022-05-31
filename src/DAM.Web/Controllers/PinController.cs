using DAM.Application;
using DAM.Application.Pin.Dtos;
using DAM.Application.Users.Models;
using DAM.Domain.Entities.Identity;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static DAM.Application.Pin.Requests.PinAssetRequest;
using static DAM.Application.Pin.Requests.PinFolderRequest;

namespace DAM.Web.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PinController : ControllerBase
    {
        private IMediator _mediator;
        private readonly UserManager<ApplicationUser> _userManager;

        public PinController(IMediator mediator, UserManager<ApplicationUser> userManager)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
            _userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
        }
//**********************************************************************************Pin Assets***************************************************************************************************//

        [ProducesResponseType(typeof(HandlerResult<List<PinAssetsDto>>), StatusCodes.Status200OK)]
        [Authorize(AuthenticationSchemes = JwtConfigConstants.AuthSchemes)]
        [HttpGet("Assets")]
        public async Task<IActionResult> GetPinAssets()
        {
            var userId = _userManager.GetUserId(User);
            var result = await _mediator.Send(new GetPinAssetsRequest(userId));

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

        [ProducesResponseType(typeof(HandlerResult<PinAssetsDto>), StatusCodes.Status201Created)]
        [DisableRequestSizeLimit]
        [Authorize(AuthenticationSchemes = JwtConfigConstants.AuthSchemes)]
        [HttpPut("Assets")]
        public async Task<IActionResult> AddPinAssets(PinAssetsDto asset)
        {
            var userId = _userManager.GetUserId(User);
            asset.UserId = userId;
            var result = await _mediator.Send(new AddPinAssetsRequest(asset));

            if (result.ResultType == ResultType.NoData)
            {
                return NotFound(new { result.Message });
            }
            if (result.ResultType == ResultType.BadRequest)
            {
                return BadRequest(new { result.Message });
            }
            if (result.ResultType == ResultType.Fail)
            {
                return Ok(new 
                {
                    Assets = result.Entity,
                    Message = "full of 12 pin assets list."
                });
            }

            return Ok(new
            {
                Assets = result.Entity
            });
        }

        [ProducesResponseType(typeof(HandlerResult<PinAssetsDto>), StatusCodes.Status201Created)]
        [DisableRequestSizeLimit]
        [Authorize(AuthenticationSchemes = JwtConfigConstants.AuthSchemes)]
        [HttpPut("Assets/Remove")]
        public async Task<IActionResult> RemovePinAssets(PinAssetsDto asset)
        {
            var userId = _userManager.GetUserId(User);
            asset.UserId = userId;
            var result = await _mediator.Send(new RemovePinAssetsRequest(asset));

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

        [ProducesResponseType(typeof(HandlerResult<List<PinAssetsDto>>), StatusCodes.Status200OK)]
        [Authorize(AuthenticationSchemes = JwtConfigConstants.AuthSchemes)]
        [HttpGet("Assets/Detail")]
        public async Task<IActionResult> GetPinAssetsDetail()
        {
            var userId = _userManager.GetUserId(User);
            
            var result = await _mediator.Send(new GetPinAssetsDetailRequest(userId));

            if (result.ResultType == ResultType.NoData)
            {
                return Ok();
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

        [ProducesResponseType(typeof(HandlerResult<PinAssetsDto>), StatusCodes.Status201Created)]
        [DisableRequestSizeLimit]
        [Authorize(AuthenticationSchemes = JwtConfigConstants.AuthSchemes)]
        [HttpPut("Assets/Replace")]
        public async Task<IActionResult> ReplacePinAssets(PinAssetsDto asset)
        {
            var userId = _userManager.GetUserId(User);
            asset.UserId = userId;
            asset.AssetId = asset.PreAssetId;
            var removeResult = await _mediator.Send(new RemovePinAssetsRequest(asset));

            if (removeResult.ResultType == ResultType.NoData)
            {
                return NotFound(new { removeResult.Message });
            }
            if (removeResult.ResultType == ResultType.BadRequest || removeResult.ResultType == ResultType.Fail)
            {
                return BadRequest(new { removeResult.Message });
            }

            asset.AssetId = asset.NewAssetId;
            var addResult = await _mediator.Send(new AddPinAssetsRequest(asset));

            if (addResult.ResultType == ResultType.NoData)
            {
                return NotFound(new { addResult.Message });
            }
            if (addResult.ResultType == ResultType.BadRequest)
            {
                return BadRequest(new { addResult.Message });
            }
            if (addResult.ResultType == ResultType.Fail)
            {
                return Ok(new
                {
                    Assets = addResult.Entity,
                    Message = "full of 12 pin assets list."
                });
            }

            return Ok(new
            {
                Assets = addResult.Entity
            });
        }

        [ProducesResponseType(typeof(HandlerResult<PinAssetsDto>), StatusCodes.Status201Created)]
        [DisableRequestSizeLimit]
        [Authorize(AuthenticationSchemes = JwtConfigConstants.AuthSchemes)]
        [HttpPut("Assets/Order")]
        public async Task<IActionResult> OrderPinAssets(PinAssetsDto asset)
        {
            var userId = _userManager.GetUserId(User);
            asset.UserId = userId;
            var result = await _mediator.Send(new OrderPinAssetsRequest(asset));

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

//**********************************************************************************Pin Folder***************************************************************************************************//

        [ProducesResponseType(typeof(HandlerResult<List<PinFoldersDto>>), StatusCodes.Status200OK)]
        [Authorize(AuthenticationSchemes = JwtConfigConstants.AuthSchemes)]
        [HttpGet("Folders")]
        public async Task<IActionResult> GetPinFolders()
        {
            var userId = _userManager.GetUserId(User);
            var result = await _mediator.Send(new GetPinFoldersRequest(userId));

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

        [ProducesResponseType(typeof(HandlerResult<PinFoldersDto>), StatusCodes.Status201Created)]
        [DisableRequestSizeLimit]
        [Authorize(AuthenticationSchemes = JwtConfigConstants.AuthSchemes)]
        [HttpPut("Folders")]
        public async Task<IActionResult> AddPinFolders(PinFoldersDto folder)
        {
            var userId = _userManager.GetUserId(User);
            folder.UserId = userId;
            var result = await _mediator.Send(new AddPinFoldersRequest(folder));

            if (result.ResultType == ResultType.NoData)
            {
                return NotFound(new { result.Message });
            }
            if (result.ResultType == ResultType.BadRequest)
            {
                return BadRequest(new { result.Message });
            }
            if (result.ResultType == ResultType.Fail)
            {
                return Ok(new 
                {
                    Folders = result.Entity,
                    Message = "full of 12 pin folders list."
                });
            }

            return Ok(new
            {
                Folders = result.Entity
            });
        }

        [ProducesResponseType(typeof(HandlerResult<PinFoldersDto>), StatusCodes.Status201Created)]
        [DisableRequestSizeLimit]
        [Authorize(AuthenticationSchemes = JwtConfigConstants.AuthSchemes)]
        [HttpPut("Folders/Remove")]
        public async Task<IActionResult> RemovePinFolders(PinFoldersDto folder)
        {
            var userId = _userManager.GetUserId(User);
            folder.UserId = userId;
            var result = await _mediator.Send(new RemovePinFoldersRequest(folder));

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

        [ProducesResponseType(typeof(HandlerResult<List<PinFoldersDto>>), StatusCodes.Status200OK)]
        [Authorize(AuthenticationSchemes = JwtConfigConstants.AuthSchemes)]
        [HttpGet("Folders/Detail")]
        public async Task<IActionResult> GetPinFoldersDetail()
        {
            var userId = _userManager.GetUserId(User);
            
            var result = await _mediator.Send(new GetPinFoldersDetailRequest(userId));

            if (result.ResultType == ResultType.NoData)
            {
                return Ok();
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

        [ProducesResponseType(typeof(HandlerResult<PinFoldersDto>), StatusCodes.Status201Created)]
        [DisableRequestSizeLimit]
        [Authorize(AuthenticationSchemes = JwtConfigConstants.AuthSchemes)]
        [HttpPut("Folders/Replace")]
        public async Task<IActionResult> ReplacePinFolders(PinFoldersDto folder)
        {
            var userId = _userManager.GetUserId(User);
            folder.UserId = userId;
            folder.FolderId = folder.PreFolderId;
            var removeResult = await _mediator.Send(new RemovePinFoldersRequest(folder));

            if (removeResult.ResultType == ResultType.NoData)
            {
                return NotFound(new { removeResult.Message });
            }
            if (removeResult.ResultType == ResultType.BadRequest || removeResult.ResultType == ResultType.Fail)
            {
                return BadRequest(new { removeResult.Message });
            }

            folder.FolderId = folder.NewFolderId;
            var addResult = await _mediator.Send(new AddPinFoldersRequest(folder));

            if (addResult.ResultType == ResultType.NoData)
            {
                return NotFound(new { addResult.Message });
            }
            if (addResult.ResultType == ResultType.BadRequest)
            {
                return BadRequest(new { addResult.Message });
            }
            if (addResult.ResultType == ResultType.Fail)
            {
                return Ok(new
                {
                    Folders = addResult.Entity,
                    Message = "full of 12 pin folders list."
                });
            }

            return Ok(new
            {
                Folders = addResult.Entity
            });
        }

        [ProducesResponseType(typeof(HandlerResult<PinFoldersDto>), StatusCodes.Status201Created)]
        [DisableRequestSizeLimit]
        [Authorize(AuthenticationSchemes = JwtConfigConstants.AuthSchemes)]
        [HttpPut("Folders/Order")]
        public async Task<IActionResult> OrderPinFolders(PinFoldersDto folder)
        {
            var userId = _userManager.GetUserId(User);
            folder.UserId = userId;
            var result = await _mediator.Send(new OrderPinFoldersRequest(folder));

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
