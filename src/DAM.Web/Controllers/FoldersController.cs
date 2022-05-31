using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using DAM.Application;
using DAM.Application.Assets.Requests;
using DAM.Application.Folders.Dtos;
using DAM.Application.Folders.Requests;
using DAM.Application.Users.Models;
using DAM.Domain.Entities.Identity;
using Ionic.Zip;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Net.Http.Headers;
using Newtonsoft.Json;
using static DAM.Application.Folders.Requests.FoldersRequest;

namespace DAM.Web.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FoldersController : ControllerBase
    {
        private IMediator _mediator;
        private readonly UserManager<ApplicationUser> _userManager;

        public FoldersController(IMediator mediator, UserManager<ApplicationUser> userManager)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
            _userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
        }

        [ProducesResponseType(typeof(HandlerResult<List<FolderDto>>), StatusCodes.Status200OK)]
        [Authorize(AuthenticationSchemes = JwtConfigConstants.AuthSchemes)]
        [HttpGet]
        public async Task<IActionResult> GetFolders()
        {

            var userId = _userManager.GetUserId(User);

            var user = await _userManager.FindByIdAsync(userId);

            if (!user.Active)
            {
                return Unauthorized();
            }

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

        [ProducesResponseType(typeof(HandlerResult<List<FolderDto>>), StatusCodes.Status200OK)]
        [Authorize(AuthenticationSchemes = JwtConfigConstants.AuthSchemes)]
        [HttpGet]
        [Route("{id}")]
        public async Task<IActionResult> GetFolders(int id)
        {
            var result = await _mediator.Send(new GetFolderRequest(id));

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


        [ProducesResponseType(typeof(HandlerResult<List<FolderDto>>), StatusCodes.Status201Created)]
        [Authorize(AuthenticationSchemes = JwtConfigConstants.AuthSchemes)]
        [HttpPut]
        [Route("Add")]
        public async Task<IActionResult> AddFolder(FolderDto folder)
        {
            var userId = _userManager.GetUserId(User);
            var result = await _mediator.Send(new AddFolderRequest(folder, userId));

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
                Folder = result.Entity,
            });
        }

        [ProducesResponseType(typeof(HandlerResult<List<FolderDto>>), StatusCodes.Status200OK)]
        [Authorize(AuthenticationSchemes = JwtConfigConstants.AuthSchemes)]
        [HttpPut]
        [Route("Update")]
        public async Task<IActionResult> UpdateFolder(FolderDto folder)
        {
            var userId = _userManager.GetUserId(User);
            var result = await _mediator.Send(new UpdateFolderRequest(folder, userId));

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
                Folder = result.Entity,
            });

        }

        [ProducesResponseType(typeof(HandlerResult<List<MoveFolderDto>>), StatusCodes.Status200OK)]
        [Authorize(AuthenticationSchemes = JwtConfigConstants.AuthSchemes)]
        [HttpPut]
        [Route("Move")]
        public async Task<IActionResult> MoveFolder(MoveFolderDto folder)
        {
            var userId = _userManager.GetUserId(User);
            var result = await _mediator.Send(new MoveFolderRequest(folder, userId));

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
                Folder = result.Entity,
            });
        }

        [ProducesResponseType(typeof(HandlerResult<List<FolderDto>>), StatusCodes.Status200OK)]
        [Authorize(AuthenticationSchemes = JwtConfigConstants.AuthSchemes)]
        [HttpPut]
        [Route("Delete")]
        public async Task<IActionResult> DeleteFolder(DeleteFoldersDto folders)
        {
            var userId = _userManager.GetUserId(User);
            var result = await _mediator.Send(new DeleteFolderRequest(folders, userId));


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

        [ProducesResponseType(typeof(HandlerResult<List<FolderDto>>), StatusCodes.Status200OK)]
        [HttpGet]
        [Route("Download/{folderId}/{userId}/{showWaterMark}")]
        public async Task<IActionResult> DownloadFolder(string folderId, string userId, bool showWaterMark)
        {
            if (string.IsNullOrEmpty(folderId))
            {
                return BadRequest("No ids");
            }

            //get all assets id under the folder
            var assets = await _mediator.Send(new GetFoldersAllAssetsRequest(folderId, userId, showWaterMark));

            if (assets.Entity.Count == 0)
            {
                return BadRequest("No data");
            }

            var result = new HttpResponseMessage()
            {
                Content = new PushStreamContent(async (outputStream, httpContext, transportContext) =>
                {
                    using (var zipStream = new ZipOutputStream(outputStream))
                    {
                        foreach (var kvp in assets.Entity)
                        {
                            zipStream.PutNextEntry(kvp.Key);
                            using (var stream = kvp.Value)
                                await stream.CopyToAsync(zipStream);
                        }
                    }
                })
            };
            return new FileStreamResult(result.Content.ReadAsStreamAsync().Result, MediaTypeHeaderValue.Parse("application/octet-stream"))
            {
                FileDownloadName = "MyZipfile.zip"
            };
        }

        [ProducesResponseType(typeof(HandlerResult<List<MoveFolderDto>>), StatusCodes.Status200OK)]
        [Authorize(AuthenticationSchemes = JwtConfigConstants.AuthSchemes)]
        [HttpPut]
        [Route("BulkMove")]
        public async Task<IActionResult> BulkMoveFolder(List<MoveFolderDto> folders)
        {
            var userId = _userManager.GetUserId(User);
            var result = await _mediator.Send(new BulkMoveFolderRequest(folders, userId));

            if (folders.Count <= 0 || result.ResultType == ResultType.NoData)
            {
                return NotFound(new { result.Message });
            }
            if (result.ResultType == ResultType.BadRequest || result.ResultType == ResultType.Fail)
            {
                return BadRequest(new { result.Message });
            }

            return Ok("Updated");
        }

        [ProducesResponseType(typeof(HandlerResult<List<MoveFolderDto>>), StatusCodes.Status200OK)]
        [Authorize(AuthenticationSchemes = JwtConfigConstants.AuthSchemes)]
        [HttpPut]
        [Route("Order")]
        public async Task<IActionResult> OrderFolders(List<MoveFolderDto> folders)
        {
            var userId = _userManager.GetUserId(User);
            var result = await _mediator.Send(new OrderFoldersRequest(folders, userId));

            if (folders.Count <= 0 || result.ResultType == ResultType.NoData)
            {
                return NotFound(new { result.Message });
            }
            if (result.ResultType == ResultType.BadRequest || result.ResultType == ResultType.Fail)
            {
                return BadRequest(new { result.Message });
            }

            return Ok("Updated");
        }

        [ProducesResponseType(typeof(HandlerResult<List<MoveFolderDto>>), StatusCodes.Status200OK)]
        [Authorize(AuthenticationSchemes = JwtConfigConstants.AuthSchemes)]
        [HttpPut]
        [Route("Copy")]
        public async Task<IActionResult> CopyFolder(CopyFolderDto folder)
        {
            var userId = _userManager.GetUserId(User);
            var result = await _mediator.Send(new CopyFolderRequest(folder, userId));

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
                Folder = result.Entity,
            });
        }
    }
}
