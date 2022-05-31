using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DAM.Application;
using DAM.Application.DownloadPDF.dto;
using DAM.Application.DownloadPDF.request;
using MediatR;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace DAM.Web.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DownloadPDFController : Controller
    {
        private IMediator _mediator;

        public DownloadPDFController(IMediator mediator)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));

        }

        [ProducesResponseType(typeof(HandlerResult<FileResult>), StatusCodes.Status200OK)]
        [HttpGet, HttpPost]
        [Route("Download")]
        public async Task<IActionResult> Download(DownloadPDFDto downloadPDFDto)
        {
            var result = await _mediator.Send(new DownloadPDFRequests(downloadPDFDto));


            return File(result.Entity.OutputStream, result.Entity.ContentType, result.Entity.FileName);

        }
    }
}
