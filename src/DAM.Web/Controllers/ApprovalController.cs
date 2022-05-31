using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DAM.Application;
using DAM.Application.Approval.Dtos;
using DAM.Application.Approvals.Dtos;
using DAM.Application.Approvals.Requests;
using DAM.Application.Assets.Dtos;
using DAM.Application.Users.Models;
using DAM.Domain.Entities;
using DAM.Domain.Entities.Identity;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace DAM.Web.Controllers
{
    [Route("/api/[controller]")]
    [ApiController]

    public class ApprovalController : ControllerBase
    {
        private IMediator _mediator;
        private UserManager<ApplicationUser> _userManager;

        public ApprovalController(IMediator mediator, UserManager<ApplicationUser> userManager)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
            _userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
        }
  
        [ProducesResponseType(typeof(HandlerResult<List<ApprovalLevelDto>>), StatusCodes.Status200OK)]
        [Authorize(AuthenticationSchemes = JwtConfigConstants.AuthSchemes)]
        [HttpGet]
        [Route("{id}/{verId}")]
        public async Task<IActionResult> GetApproval(int id, int verId)
        {
            var result = await _mediator.Send(new GetAssetApprovalsRequest(id, verId));


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
                ApprovalLevels = result.Entity,
            });
        }

        [ProducesResponseType(typeof(HandlerResult<AssetApprovalDto>), StatusCodes.Status200OK)]
        [Authorize(AuthenticationSchemes = JwtConfigConstants.AuthSchemes)]
        [HttpPut]
        [Route("Save")]
        public async Task<IActionResult> SaveApproval(AssetApprovalDto assetApproval)
        {
            var userId = _userManager.GetUserId(User);
            var result = await _mediator.Send(new SaveApprovalRequest(assetApproval, userId));


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
                Approvals = result.Entity,
            });
        }

        [ProducesResponseType(typeof(HandlerResult<AssetApprovalDto>), StatusCodes.Status200OK)]
        [Authorize(AuthenticationSchemes = JwtConfigConstants.AuthSchemes)]
        [HttpPut]
        [Route("Update")]
        public async Task<IActionResult> UpdateApproval(List<UpdateApprovalDto> assetApprovals)
        {
            var userId = _userManager.GetUserId(User);
            var result = await _mediator.Send(new UpdateApprovalRequest(assetApprovals, userId));


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
                Approvals = result.Entity,
            });
        }


        [ProducesResponseType(typeof(HandlerResult<List<ApprovalLevelDto>>), StatusCodes.Status200OK)]
        [Authorize(AuthenticationSchemes = JwtConfigConstants.AuthSchemes)]
        [HttpGet]
        [Route("Templates/{id}")]
        public async Task<IActionResult> GetApprovalTemplates(int id)
        {
            var result = await _mediator.Send(new GetApprovalTemplatesRequest(id));

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
                ApprovalTemplates = result.Entity,
            });
        }

        [ProducesResponseType(typeof(HandlerResult<List<ApprovalLevelDto>>), StatusCodes.Status200OK)]
        [Authorize(AuthenticationSchemes = JwtConfigConstants.AuthSchemes)]
        [HttpPut]
        [Route("UpdateTemplate")]
        public async Task<IActionResult> UpdateApprovalTemplate(ApprovalTemplateDto approvalTemplate)
        {
            var userId = _userManager.GetUserId(User);
            var result = await _mediator.Send(new UpdateApprovalTemplateRequest(approvalTemplate, userId));

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
                ApprovalTemplates = result.Entity,
            });
        }


        [ProducesResponseType(typeof(HandlerResult<List<ApprovalLevelDto>>), StatusCodes.Status200OK)]
        [Authorize(AuthenticationSchemes = JwtConfigConstants.AuthSchemes)]
        [HttpPut]
        [Route("CreateTemplate")]
        public async Task<IActionResult> CreateApprovalTemplate(ApprovalTemplateDto approvalTemplate)
        {
            var userId = _userManager.GetUserId(User);
            var result = await _mediator.Send(new CreateApprovalTemplateRequest(approvalTemplate, userId));

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
                ApprovalTemplates = result.Entity,
            });
        }

        [ProducesResponseType(typeof(HandlerResult<List<ApprovalLevelDto>>), StatusCodes.Status200OK)]
        [Authorize(AuthenticationSchemes = JwtConfigConstants.AuthSchemes)]
        [HttpPut]
        [Route("DeleteTemplate")]
        public async Task<IActionResult> DeleteApprovalTemplate(ApprovalTemplateDto approvalTemplate)
        {
            var userId = _userManager.GetUserId(User);
            var result = await _mediator.Send(new DeleteApprovalTemplateRequest(approvalTemplate, userId));

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
                ApprovalTemplates = result.Entity,
            });
        }

        [ProducesResponseType(typeof(HandlerResult<IEnumerable<ApprovalsOnOOODto>>), StatusCodes.Status200OK)]
        //[Authorize(AuthenticationSchemes = JwtConfigConstants.AuthSchemes)]
        [HttpPost]
        [Route("ApprovalsOnOOO")]
        //public async Task<IActionResult> GetApprovalsOnOOO([FromQuery] string approverId, [FromQuery] string startDate, [FromQuery] string endDate)
        public async Task<IActionResult> GetApprovalsOnOOO(GetApprovalsOnOOODto approvalsOnOOODto)
        {
            //var input = new GetApprovalsOnOOODto()
            //{
            //    ApproverId = approverId,
            //    StartDate = Convert.ToDateTime(startDate),
            //    EndDate = endDate
            //};

            var result = await _mediator.Send(new GetApprovalsOnOOORequest(approvalsOnOOODto));

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
                AssetsForApproval = result.Entity,
            });
        }

        [ProducesResponseType(typeof(HandlerResult<DelegateAssetsForApprovalDto>), StatusCodes.Status200OK)]
        [Authorize(AuthenticationSchemes = JwtConfigConstants.AuthSchemes)]
        [HttpPost]
        [Route("Delegate")]
        public async Task<IActionResult> DelegateApprovals(DelegateAssetsForApprovalDto delegateAssets)
        {
            var result = await _mediator.Send(new DelegateAssetsForApprovalRequest(delegateAssets));

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
                DelegatedAssetsForApproval = result.Entity,
            });
        }

        [ProducesResponseType(typeof(HandlerResult<List<AssetDto>>), StatusCodes.Status200OK)]
        [Authorize(AuthenticationSchemes = JwtConfigConstants.AuthSchemes)]
        [HttpGet("MySentApproval")]
        public async Task<IActionResult> GetMySentApprovalAssetsDetail()
        {
            var userId = _userManager.GetUserId(User);

            var result = await _mediator.Send(new GetMySentApprovalAssetsRequest(userId));

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
    }
}
