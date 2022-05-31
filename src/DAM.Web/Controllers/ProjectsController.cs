using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DAM.Application;
using DAM.Application.Carts.Dtos;
using DAM.Application.Carts.Requests;
using DAM.Application.Projects.Dtos;
using DAM.Application.Projects.Requests;
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
    public class ProjectsController : ControllerBase
    {
        private IMediator _mediator;
        private readonly UserManager<ApplicationUser> _userManager;

        public ProjectsController(IMediator mediator, UserManager<ApplicationUser> userManager)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
            _userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
        }

        //[ProducesResponseType(typeof(HandlerResult<ProjectDto>), StatusCodes.Status200OK)]
        //[Authorize(AuthenticationSchemes = JwtConfigConstants.AuthSchemes)]
        //[DisableRequestSizeLimit]
        //[HttpPut]
        //[Route("Delete")]
        //public async Task<IActionResult> DeleteProject(ProjectDto projectDto)
        //{
        //    var result = await _mediator.Send(new DeleteCartRequest(projectDto));

        //    if (result.ResultType == ResultType.BadRequest || result.ResultType == ResultType.Fail)
        //    {
        //        return BadRequest(new { result.Message });
        //    }

        //    return Ok("OK");
        //}

        [ProducesResponseType(typeof(HandlerResult<List<ProjectDto>>), StatusCodes.Status200OK)]
        [Authorize(AuthenticationSchemes = JwtConfigConstants.AuthSchemes)]
        [HttpGet]
        public async Task<IActionResult> GetAllUserProjects()
        {
            var userId = _userManager.GetUserId(User);

            var result = await _mediator.Send(new ProjectsRequest(0, userId));

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
                Projects = result.Entity
            });
        }

        [ProducesResponseType(typeof(HandlerResult<List<ProjectDto>>), StatusCodes.Status200OK)]
        [Authorize(AuthenticationSchemes = JwtConfigConstants.AuthSchemes)]
        [Route("Archived")]
        [HttpGet]
        public async Task<IActionResult> GetAllUserProjectsArchived()
        {
            var userId = _userManager.GetUserId(User);

            var result = await _mediator.Send(new ProjectsRequest(0, userId, true));

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
                Projects = result.Entity
            });
        }

        [ProducesResponseType(typeof(HandlerResult<List<ProjectDto>>), StatusCodes.Status200OK)]
        [Authorize(AuthenticationSchemes = JwtConfigConstants.AuthSchemes)]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetProject(int id)
        {
            var result = await _mediator.Send(new GetProjectAssetsRequest(id));

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
                Project = result.Entity
            });
        }

        [ProducesResponseType(typeof(HandlerResult<ProjectFollowersDto>), StatusCodes.Status200OK)]
        [Authorize(AuthenticationSchemes = JwtConfigConstants.AuthSchemes)]
        [HttpGet]
        [Route("Owners/{id}")]
        public async Task<IActionResult> GetProjectOwners(int id)
        {
            var result = await _mediator.Send(new ProjectOwnersRequest(id));

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
                Owners = result.Entity
            });
        }

        [ProducesResponseType(typeof(HandlerResult<ProjectFollowersDto>), StatusCodes.Status200OK)]
        [Authorize(AuthenticationSchemes = JwtConfigConstants.AuthSchemes)]
        [HttpGet]
        [Route("Followers/{id}")]
        public async Task<IActionResult> GetProjectFollowers(int id)
        {
            var result = await _mediator.Send(new ProjectFollowersRequest(id));

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
                Followers = result.Entity
            });
        }

        [ProducesResponseType(typeof(HandlerResult<List<ProjectCommentDto>>), StatusCodes.Status200OK)]
        [Authorize(AuthenticationSchemes = JwtConfigConstants.AuthSchemes)]
        [HttpGet]
        [Route("Comments/{id}")]
        public async Task<IActionResult> GetProjectComments(int id)
        {
            var result = await _mediator.Send(new ProjectCommentsRequest(id));

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
                Comments = result.Entity
            });
        }

        [ProducesResponseType(typeof(HandlerResult<List<ProjectCommentDto>>), StatusCodes.Status200OK)]
        [Authorize(AuthenticationSchemes = JwtConfigConstants.AuthSchemes)]
        [HttpPost]
        [Route("Comments/Delete")]
        public async Task<IActionResult> DeleteProjectComment(DeleteProjectCommentDto deleteComment)
        {
            var userId = _userManager.GetUserId(User);
            var result = await _mediator.Send(new DeleteProjectCommentRequest(deleteComment, userId));

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
                DeletedComment = result.Entity
            });
        }

        [ProducesResponseType(typeof(HandlerResult<ProjectDto>), StatusCodes.Status201Created)]
        [Authorize(AuthenticationSchemes = JwtConfigConstants.AuthSchemes)]
        [HttpPost]
        [Route("Save")]
        public async Task<IActionResult> SaveProject(ProjectDto projectDto)
        {
            var userId = _userManager.GetUserId(User);

            var result = await _mediator.Send(new SaveProjectRequest(projectDto, userId));

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
                Project = result.Entity
            });
        }

        [ProducesResponseType(typeof(HandlerResult<ArchiveProjectDto>), StatusCodes.Status201Created)]
        [Authorize(AuthenticationSchemes = JwtConfigConstants.AuthSchemes)]
        [HttpPost]
        [Route("Archive")]
        public async Task<IActionResult> ArchiveProject(ArchiveProjectDto projectDto)
        {
            var userId = _userManager.GetUserId(User);

            var result = await _mediator.Send(new ArchiveProjectRequest(projectDto.ProjectId, userId));

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
                Project = result.Entity
            });
        }


        [ProducesResponseType(typeof(HandlerResult<ArchiveProjectDto>), StatusCodes.Status201Created)]
        [Authorize(AuthenticationSchemes = JwtConfigConstants.AuthSchemes)]
        [HttpPost]
        [Route("Unarchive")]
        public async Task<IActionResult> UnarchiveProject(ArchiveProjectDto projectDto)
        {
            var userId = _userManager.GetUserId(User);

            var result = await _mediator.Send(new UnarchiveProjectRequest(projectDto.ProjectId, userId));

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
                Project = result.Entity
            });
        }

        [ProducesResponseType(typeof(HandlerResult<ArchiveProjectDto>), StatusCodes.Status201Created)]
        [Authorize(AuthenticationSchemes = JwtConfigConstants.AuthSchemes)]
        [HttpPost]
        [Route("Delete")]
        public async Task<IActionResult> DeleteProject(ArchiveProjectDto projectDto)
        {
            var userId = _userManager.GetUserId(User);

            var result = await _mediator.Send(new DeleteProjectRequest(projectDto.ProjectId, userId));

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
                Project = result.Entity
            });
        }

        [ProducesResponseType(typeof(HandlerResult<ProjectCommentDto>), StatusCodes.Status201Created)]
        [Authorize(AuthenticationSchemes = JwtConfigConstants.AuthSchemes)]
        [HttpPost]
        [Route("Comments/Save")]
        public async Task<IActionResult> AddComment(ProjectCommentDto projectCommentDto)
        {
            var userId = _userManager.GetUserId(User);

            var result = await _mediator.Send(new SaveProjectCommentRequest(projectCommentDto, userId));

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
                ProjectComment = result.Entity
            });
        }

        [ProducesResponseType(typeof(HandlerResult<ImportAssetsToProjectDto>), StatusCodes.Status201Created)]
        [Authorize(AuthenticationSchemes = JwtConfigConstants.AuthSchemes)]
        [HttpPost]
        [Route("Assets/Import")]
        public async Task<IActionResult> ImportAssetsToProject(ImportAssetsToProjectDto importDto)
        {
            var userId = _userManager.GetUserId(User);

            var result = await _mediator.Send(new ImportAssetsToProjectRequest(importDto, userId));

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
                ImportedAssets = result.Entity
            });
        }

        [ProducesResponseType(typeof(HandlerResult<IEnumerable<ProjectItemDto>>), StatusCodes.Status201Created)]
        [Authorize(AuthenticationSchemes = JwtConfigConstants.AuthSchemes)]
        [HttpGet]
        [Route("Assets/Import/{id}")]
        public async Task<IActionResult> GetImportedProjectAssets(int id)
        {
            var userId = _userManager.GetUserId(User);

            var result = await _mediator.Send(new GetImportedProjectAssetsRequest(id));

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
                ImportedAssets = result.Entity
            });
        }

        [ProducesResponseType(typeof(HandlerResult<IEnumerable<ProjectItemDto>>), StatusCodes.Status201Created)]
        [Authorize(AuthenticationSchemes = JwtConfigConstants.AuthSchemes)]
        [HttpGet]
        [Route("Assets/Uploads/{id}")]
        public async Task<IActionResult> GetProjectUploads(int id)
        {
            var userId = _userManager.GetUserId(User);

            var result = await _mediator.Send(new GetProjectUploadsRequest(id));

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
                ProjectUploads = result.Entity
            });
        }

        [ProducesResponseType(typeof(HandlerResult<ImportAssetsToProjectDto>), StatusCodes.Status201Created)]
        [Authorize(AuthenticationSchemes = JwtConfigConstants.AuthSchemes)]
        [HttpPost]
        [Route("Assets/Remove")]
        public async Task<IActionResult> RemoveAssetsFromProject(ImportAssetsToProjectDto importDto)
        {
            var userId = _userManager.GetUserId(User);

            var result = await _mediator.Send(new RemoveAssetsFromProjectRequest(importDto, userId));

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
                RemovedAssets = result.Entity
            });
        }
    }
}
