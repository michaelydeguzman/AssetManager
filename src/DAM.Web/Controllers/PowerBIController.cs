using DAM.Application;
using DAM.Application.Services.Interfaces;
using DAM.Domain.Entities;
using DAM.Domain.Entities.Identity;
using DAM.Persistence;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;
using System.Threading.Tasks;
using static DAM.Application.PwoerBI.Requests.PowerBIRequest;
namespace DAM.Web.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PowerBIController : ControllerBase
    {
        private IMediator _mediator;
        private readonly IDbContext _dbcontext;
        private readonly IEmailService _emailService;
        private IConfiguration _configuration;
        private readonly UserManager<ApplicationUser> _userManager;
        private string _baseUrl;

        public PowerBIController(IMediator mediator, UserManager<ApplicationUser> userManager, IDbContext dbcontext, IEmailService emailService, IConfiguration configuration)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
            _userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
            _dbcontext = dbcontext ?? throw new ArgumentNullException(nameof(dbcontext));
            _emailService = emailService ?? throw new ArgumentNullException(nameof(emailService));
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
            _baseUrl = _configuration["BaseUrl"];
        }

        [ProducesResponseType(typeof(HandlerResult<Asset>), StatusCodes.Status200OK)]
        [HttpGet]
        [Route("Assets")]//this means the final url is localhost:1632/api/PowerBI/Assets, domain name + api/[controller] + Route value; api/[controller] is line 21, [controller] means line 23 controller name expcept the word controller
        public async Task<IActionResult> GetAssetsTable() // ideally using get + table name + table to identify it easily
        {
            var result = await _mediator.Send(new GetAssetTableRequest()); // will invoke the request GetAssetTableRequest which is in \Simple DAM\src\DAM.Application\PowerBI\Requests\PowerBIRequests.cs
            if (result.ResultType == ResultType.NoData)
            {
                return NotFound(new { result.Message });
            }
            if (result.ResultType == ResultType.BadRequest || result.ResultType == ResultType.Fail)
            {
                return BadRequest(new { result.Message });
            }
            return Ok(new{Assets = result.Entity});
        }

        [ProducesResponseType(typeof(HandlerResult<Asset>), StatusCodes.Status200OK)]
        [HttpGet]
        [Route("AssetVersions")]
        public async Task<IActionResult> GetAssetVersionsTable()
        {
            var result = await _mediator.Send(new GetAssetVersionTableRequest());
            if (result.ResultType == ResultType.NoData)
            {
                return NotFound(new { result.Message });
            }
            if (result.ResultType == ResultType.BadRequest || result.ResultType == ResultType.Fail)
            {
                return BadRequest(new { result.Message });
            }
            return Ok(new{AssetVersions = result.Entity});
        }
        [ProducesResponseType(typeof(HandlerResult<Asset>), StatusCodes.Status200OK)]
        [HttpGet]
        [Route("ApprovalLevelApprovers")]
        public async Task<IActionResult> GetApprovalLevelApproversTable()
        {
            var result = await _mediator.Send(new GetApprovalLevelApproversTableRequest());
            if (result.ResultType == ResultType.NoData)
            {
                return NotFound(new { result.Message });
            }
            if (result.ResultType == ResultType.BadRequest || result.ResultType == ResultType.Fail)
            {
                return BadRequest(new { result.Message });
            }
            return Ok(new{ApprovalLevelApprovers = result.Entity});
        }
        [ProducesResponseType(typeof(HandlerResult<Asset>), StatusCodes.Status200OK)]
        [HttpGet]
        [Route("ApprovalLevels")]
        public async Task<IActionResult> GetApprovalLevelsTable()
        {
            var result = await _mediator.Send(new GetApprovalLevelsTableRequest());
            if (result.ResultType == ResultType.NoData)
            {
                return NotFound(new { result.Message });
            }
            if (result.ResultType == ResultType.BadRequest || result.ResultType == ResultType.Fail)
            {
                return BadRequest(new { result.Message });
            }
            return Ok(new{ApprovalLevels = result.Entity});
        }
        [ProducesResponseType(typeof(HandlerResult<Asset>), StatusCodes.Status200OK)]
        [HttpGet]
        [Route("AssetAccounts")]
        public async Task<IActionResult> GetAssetAccountsTable()
        {
            var result = await _mediator.Send(new GetAssetAccountsTableRequest());
            if (result.ResultType == ResultType.NoData)
            {
                return NotFound(new { result.Message });
            }
            if (result.ResultType == ResultType.BadRequest || result.ResultType == ResultType.Fail)
            {
                return BadRequest(new { result.Message });
            }
            return Ok(new{AssetAccounts = result.Entity});
        }
        [ProducesResponseType(typeof(HandlerResult<Asset>), StatusCodes.Status200OK)]
        [HttpGet]
        [Route("AssetCountryRegions")]
        public async Task<IActionResult> GetAssetCountryRegionsTable()
        {
            var result = await _mediator.Send(new GetAssetCountryRegionsTableRequest());
            if (result.ResultType == ResultType.NoData)
            {
                return NotFound(new { result.Message });
            }
            if (result.ResultType == ResultType.BadRequest || result.ResultType == ResultType.Fail)
            {
                return BadRequest(new { result.Message });
            }
            return Ok(new{AssetCountryRegions = result.Entity});
        }
        [ProducesResponseType(typeof(HandlerResult<Asset>), StatusCodes.Status200OK)]
        [HttpGet]
        [Route("Cart")]
        public async Task<IActionResult> GetCartTable()
        {
            var result = await _mediator.Send(new GetCartTableRequest());
            if (result.ResultType == ResultType.NoData)
            {
                return NotFound(new { result.Message });
            }
            if (result.ResultType == ResultType.BadRequest || result.ResultType == ResultType.Fail)
            {
                return BadRequest(new { result.Message });
            }
            return Ok(new{Cart = result.Entity});
        }
        [ProducesResponseType(typeof(HandlerResult<Asset>), StatusCodes.Status200OK)]
        [HttpGet]
        [Route("CartItems")]
        public async Task<IActionResult> GetCartItemsTable()
        {
            var result = await _mediator.Send(new GetCartItemsTableRequest());
            if (result.ResultType == ResultType.NoData)
            {
                return NotFound(new { result.Message });
            }
            if (result.ResultType == ResultType.BadRequest || result.ResultType == ResultType.Fail)
            {
                return BadRequest(new { result.Message });
            }
            return Ok(new{CartItems = result.Entity});
        }
        [ProducesResponseType(typeof(HandlerResult<Asset>), StatusCodes.Status200OK)]
        [HttpGet]
        [Route("Companies")]
        public async Task<IActionResult> GetCompaniesTable()
        {
            var result = await _mediator.Send(new GetCompaniesTableRequest());
            if (result.ResultType == ResultType.NoData)
            {
                return NotFound(new { result.Message });
            }
            if (result.ResultType == ResultType.BadRequest || result.ResultType == ResultType.Fail)
            {
                return BadRequest(new { result.Message });
            }
            return Ok(new{Companies = result.Entity});
        }
        [ProducesResponseType(typeof(HandlerResult<Asset>), StatusCodes.Status200OK)]
        [HttpGet]
        [Route("Countries")]
        public async Task<IActionResult> GetCountriesTable()
        {
            var result = await _mediator.Send(new GetCountriesTableRequest());
            if (result.ResultType == ResultType.NoData)
            {
                return NotFound(new { result.Message });
            }
            if (result.ResultType == ResultType.BadRequest || result.ResultType == ResultType.Fail)
            {
                return BadRequest(new { result.Message });
            }
            return Ok(new{Countries = result.Entity});
        }
        [ProducesResponseType(typeof(HandlerResult<Asset>), StatusCodes.Status200OK)]
        [HttpGet]
        [Route("FolderCountryRegions")]
        public async Task<IActionResult> GetFolderCountryRegionsTable()
        {
            var result = await _mediator.Send(new GetFolderCountryRegionsTableRequest());
            if (result.ResultType == ResultType.NoData)
            {
                return NotFound(new { result.Message });
            }
            if (result.ResultType == ResultType.BadRequest || result.ResultType == ResultType.Fail)
            {
                return BadRequest(new { result.Message });
            }
            return Ok(new{FolderCountryRegions = result.Entity});
        }
        [ProducesResponseType(typeof(HandlerResult<Asset>), StatusCodes.Status200OK)]
        [HttpGet]
        [Route("Folders")]
        public async Task<IActionResult> GetFoldersTable()
        {
            var result = await _mediator.Send(new GetFoldersTableRequest());
            if (result.ResultType == ResultType.NoData)
            {
                return NotFound(new { result.Message });
            }
            if (result.ResultType == ResultType.BadRequest || result.ResultType == ResultType.Fail)
            {
                return BadRequest(new { result.Message });
            }
            return Ok(new{Folders = result.Entity});
        }
        [ProducesResponseType(typeof(HandlerResult<Asset>), StatusCodes.Status200OK)]
        [HttpGet]
        [Route("Regions")]
        public async Task<IActionResult> GetRegionsTable()
        {
            var result = await _mediator.Send(new GetRegionsTableRequest());
            if (result.ResultType == ResultType.NoData)
            {
                return NotFound(new { result.Message });
            }
            if (result.ResultType == ResultType.BadRequest || result.ResultType == ResultType.Fail)
            {
                return BadRequest(new { result.Message });
            }
            return Ok(new{Regions = result.Entity});
        }
        [ProducesResponseType(typeof(HandlerResult<Asset>), StatusCodes.Status200OK)]
        [HttpGet]
        [Route("Tags")]
        public async Task<IActionResult> GetTagsTable()
        {
            var result = await _mediator.Send(new GetTagsTableRequest());
            if (result.ResultType == ResultType.NoData)
            {
                return NotFound(new { result.Message });
            }
            if (result.ResultType == ResultType.BadRequest || result.ResultType == ResultType.Fail)
            {
                return BadRequest(new { result.Message });
            }
            return Ok(new{Tags = result.Entity});
        }
        [ProducesResponseType(typeof(HandlerResult<Asset>), StatusCodes.Status200OK)]
        [HttpGet]
        [Route("UserFolders")]
        public async Task<IActionResult> GetUserFoldersTable()
        {
            var result = await _mediator.Send(new GetUserFoldersTableRequest());
            if (result.ResultType == ResultType.NoData)
            {
                return NotFound(new { result.Message });
            }
            if (result.ResultType == ResultType.BadRequest || result.ResultType == ResultType.Fail)
            {
                return BadRequest(new { result.Message });
            }
            return Ok(new{UserFolders = result.Entity});
        }
        [ProducesResponseType(typeof(HandlerResult<Asset>), StatusCodes.Status200OK)]
        [HttpGet]
        [Route("AssetAudit")]
        public async Task<IActionResult> GetAssetAuditTable()
        {
            var result = await _mediator.Send(new GetAssetAuditTableRequest());
            if (result.ResultType == ResultType.NoData)
            {
                return NotFound(new { result.Message });
            }
            if (result.ResultType == ResultType.BadRequest || result.ResultType == ResultType.Fail)
            {
                return BadRequest(new { result.Message });
            }
            return Ok(new{AssetAudit = result.Entity});
        }
        [ProducesResponseType(typeof(HandlerResult<Asset>), StatusCodes.Status200OK)]
        [HttpGet]
        [Route("AppUsers")]
        public async Task<IActionResult> GetAppUsersTable()
        {
            var result = await _mediator.Send(new GetAppUsersTableRequest());
            if (result.ResultType == ResultType.NoData)
            {
                return NotFound(new { result.Message });
            }
            if (result.ResultType == ResultType.BadRequest || result.ResultType == ResultType.Fail)
            {
                return BadRequest(new { result.Message });
            }
            return Ok(new{AppUsers = result.Entity});
        }
    }
}