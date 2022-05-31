using System;
using System.Linq;
using System.Threading.Tasks;
using DAM.Application.Assets.Dtos;
using DAM.Application.Services;
using DAM.Persistence;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace DAM.Web.Controllers
{
    [Route("wopi/[controller]")]
    [ApiController]
    public class FilesController : ControllerBase
    {
        private readonly IHttpContextAccessor _contextAccessor;
        private readonly IDbContext _dbcontext;
        private readonly IConfiguration _configuration;

        public FilesController(IHttpContextAccessor contextAccessor, IDbContext dbContext, IConfiguration configuration)
        {
            _contextAccessor = contextAccessor;
            _dbcontext = dbContext;
            _configuration = configuration;
        }

        [HttpGet]
        [Route("{id}")]
        public async Task<IActionResult> GetCheckFileInfo(string id)
        {
            WopiFileDto file = null;

            // implement validation

            // retrieve file
            //var asset = _dbcontext.Assets.FirstOrDefault(x => x.Key == id);
            var asset = _dbcontext.AssetVersions.FirstOrDefault(a => a.Key == id);


            if (asset != null)
            {
                var owner = _dbcontext.AppUsers.First(u => u.Id == asset.CreatedById);
                // Call CheckFileInfo logic
                file = new WopiFileDto()
                {
                    Id = asset.Key,
                    BaseFileName = asset.FileName,
                    OwnerId = owner.Email,
                    Size = asset.Size,
                    Version = 1
                };
            }

            return new JsonResult(file);
        }

        [HttpGet]
        [Route("{id}/contents")]
        public async Task<IActionResult> GetFileContent(string id)
        {
            // implement validation
            // retrieve file
            var asset = _dbcontext.AssetVersions.FirstOrDefault(a => a.Key == id);

            if (asset != null)
            {
                AzureStorageService azureStorageService = new AzureStorageService();
                var blobClient = azureStorageService.GetBlobFileBytes(_configuration, id + "." + asset.Extension);
                return new FileStreamResult(blobClient.OpenRead(), "application/octet-stream");
            }
            return NotFound();
        }

        [HttpPut("{id}/contents")]
        [HttpPost("{id}/contents")]
        public async Task<IActionResult> PutFileContents(string id)
        {
            //if (!(await _authorizationService.AuthorizeAsync(User, new FileResource(id), WopiOperations.Read)).Succeeded)
            //{
            //    return Unauthorized();
            //}
            var test = new Object();
            return new JsonResult(test);
        }

      
    }
}
