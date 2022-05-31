using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DAM.Application;
using DAM.Application.Assets.Dtos;
using DAM.Application.Assets.Requests;
using DAM.Application.AuditTrail.Dtos;
using DAM.Application.AuditTrail.Requests;
using DAM.Application.Templates;
using DAM.Application.Users.Dtos;
using DAM.Application.Users.Exceptions;
using DAM.Application.Users.Models;
using DAM.Application.Users.Requests;
using DAM.Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace DAM.Web.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuditTrailController : ControllerBase
    {
        private IMediator _mediator;

        public AuditTrailController(IMediator mediator)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        [ProducesResponseType(typeof(HandlerResult<AuditTrailDto>), StatusCodes.Status200OK)]
        [Authorize(AuthenticationSchemes = JwtConfigConstants.AuthSchemes)]
        [HttpGet]
        public async Task<IActionResult> GetAuditTrail([FromQuery]int currentPageNumber, [FromQuery]int sortOrder, [FromQuery]int pageSize, [FromQuery]string sortColumnName, [FromQuery]string fileName, [FromQuery]string folderName)
        {
            var fetchAuditDto = new FetchAuditDto()
            {
                CurrentPageNumber = currentPageNumber,
                SortOrder = sortOrder,
                PageSize = pageSize,
                SortColumnName = sortColumnName,
                FileName = fileName,
                FolderName = folderName
            };

            var result = await _mediator.Send(new GetAuditTrailRequest(fetchAuditDto));

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
                AuditTrail = result.Entity
            });
        }
    }
}