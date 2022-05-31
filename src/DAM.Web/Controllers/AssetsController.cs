using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using DAM.Application;
using DAM.Application.Approvals.Requests;
using DAM.Application.Assets.Dtos;
using DAM.Application.Assets.Enums;
using DAM.Application.Assets.Requests;
using DAM.Application.Users.Models;
using DAM.Domain.Entities;
using DAM.Domain.Entities.Identity;
using DAM.Persistence;
using Ionic.Zip;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Net.Http.Headers;
using Newtonsoft.Json;

namespace DAM.Web.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AssetsController : ControllerBase
    {
        private IMediator _mediator;
        private readonly IDbContext _dbcontext;
        private readonly UserManager<ApplicationUser> _userManager;
        private IConfiguration _configuration;

        public AssetsController(IMediator mediator, UserManager<ApplicationUser> userManager, IDbContext dbcontext, IConfiguration configuration)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
            _userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
            _dbcontext = dbcontext ?? throw new ArgumentNullException(nameof(dbcontext));
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
        }

        [ProducesResponseType(typeof(HandlerResult<List<AssetDto>>), StatusCodes.Status200OK)]
        [Authorize(AuthenticationSchemes = JwtConfigConstants.AuthSchemes)]
        [HttpGet]
        public async Task<IActionResult> GetAssets()
        {
            var result = await _mediator.Send(new GetAssetRequest(0));

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

        [ProducesResponseType(typeof(HandlerResult<List<AssetDto>>), StatusCodes.Status200OK)]
        [Authorize(AuthenticationSchemes = JwtConfigConstants.AuthSchemes)]
        [HttpGet]
        [Route("Approvals")]
        public async Task<IActionResult> GetAssetsForApproval()
        {
            var userId = _userManager.GetUserId(User);
            var user = await _userManager.FindByIdAsync(userId);
            //1 Administator 2 companyAdmin
            if (user.UserRoleId > 2)
            {
                return Unauthorized();
            }

            var result = await _mediator.Send(new GetAssetsForApprovalRequest(userId));

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

        [ProducesResponseType(typeof(HandlerResult<List<AssetAuditDto>>), StatusCodes.Status200OK)]
        [Authorize(AuthenticationSchemes = JwtConfigConstants.AuthSchemes)]
        [HttpGet]
        [Route("Audit")]
        public async Task<IActionResult> GetAssetsAudit()
        {
            var userId = _userManager.GetUserId(User);
            var user = await _userManager.FindByIdAsync(userId);
            //1 Administator
            if (user.UserRoleId != 1)
            {
                return Unauthorized();
            }

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

        [ProducesResponseType(typeof(HandlerResult<List<AssetDto>>), StatusCodes.Status200OK)]
        [Authorize(AuthenticationSchemes = JwtConfigConstants.AuthSchemes)]
        [HttpGet]
        [Route("Archive")]
        public async Task<IActionResult> GetArchivedAssets()
        {
            var userId = _userManager.GetUserId(User);
            var user = await _userManager.FindByIdAsync(userId);
            //1 Administator 2 company admin
            if (user.UserRoleId > 2)
            {
                return Unauthorized();
            }

            var result = await _mediator.Send(new GetArchiveRequest(0, 0));

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

        [ProducesResponseType(typeof(HandlerResult<List<AssetDto>>), StatusCodes.Status200OK)]
        [Authorize(AuthenticationSchemes = JwtConfigConstants.AuthSchemes)]
        [HttpGet]
        [Route("{id}")]
        public async Task<IActionResult> GetAsset(int id)
        {
            var result = await _mediator.Send(new GetAssetRequest(id));

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

        [ProducesResponseType(typeof(HandlerResult<AssetDto>), StatusCodes.Status201Created)]
        //[RequestSizeLimit(2_147_483_648)]
        [DisableRequestSizeLimit]
        [Authorize(AuthenticationSchemes = JwtConfigConstants.AuthSchemes)]
        [HttpPut]
        [Route("Add")]
        public async Task<IActionResult> AddAsset(AssetDto asset)
        {
            var userId = _userManager.GetUserId(User);
            var result = await _mediator.Send(new AddAssetRequest(asset, userId));

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
                Asset = result.Entity
            });
        }

        [ProducesResponseType(typeof(HandlerResult<AssetDto>), StatusCodes.Status201Created)]
        //[RequestSizeLimit(2_147_483_648)]
        [DisableRequestSizeLimit]
        [Authorize(AuthenticationSchemes = JwtConfigConstants.AuthSchemes)]
        [HttpPut]
        [Route("AddVersion")]
        public async Task<IActionResult> AddAsseVersion(AssetDto asset)
        {
            var userId = _userManager.GetUserId(User);
            var result = await _mediator.Send(new AddAssetVersionRequest(asset, userId));

            if (result.ResultType == ResultType.NoData)
            {
                return NotFound(new { result.Message });
            }
            if (result.ResultType == ResultType.BadRequest || result.ResultType == ResultType.Fail)
            {
                return BadRequest(new { result.Message });
            }

            var obj = JsonConvert.SerializeObject(result.Entity, Formatting.Indented, new JsonSerializerSettings { ReferenceLoopHandling = ReferenceLoopHandling.Ignore });

            return Ok(new
            {
                Asset = obj,
            });
        }


        [ProducesResponseType(typeof(HandlerResult<AssetDto>), StatusCodes.Status201Created)]
        //[RequestSizeLimit(2_147_483_648)]
        [DisableRequestSizeLimit]
        [Authorize(AuthenticationSchemes = JwtConfigConstants.AuthSchemes)]
        [HttpGet]
        [Route("Revert/{assetID}/{versionID}")]
        public async Task<IActionResult> RevertAsset(int assetID, int versionID)
        {
            var userId = _userManager.GetUserId(User);
            var result = await _mediator.Send(new RevertAssetRequest(assetID, versionID));

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
                Asset = result.Entity,
            });
        }


        [ProducesResponseType(typeof(HandlerResult<AssetDto>), StatusCodes.Status201Created)]
        //[RequestSizeLimit(2_147_483_648)]
        [DisableRequestSizeLimit]
        [Authorize(AuthenticationSchemes = JwtConfigConstants.AuthSchemes)]
        [HttpGet]
        [Route("GetAssetVersion/{assetId}/{versionId}")]
        public async Task<IActionResult> GetAssetVersion(int assetID, int versionID)
        {
            var userId = _userManager.GetUserId(User);
            var result = await _mediator.Send(new GetAssetVersionRequest(assetID, versionID));

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
                Asset = result.Entity,
            });
        }

        [ProducesResponseType(typeof(HandlerResult<AssetDto>), StatusCodes.Status200OK)]
        [Authorize(AuthenticationSchemes = JwtConfigConstants.AuthSchemes)]
        [HttpPut]
        [Route("Update")]
        public async Task<IActionResult> UpdateAsset(UpdateAssetDto asset)
        {
            var userId = _userManager.GetUserId(User);
            var result = await _mediator.Send(new UpdateAssetRequest(asset, userId));

            if (result.ResultType == ResultType.NoData)
            {
                return NotFound(new { result.Message });
            }
            if (result.ResultType == ResultType.BadRequest || result.ResultType == ResultType.Fail)
            {
                return BadRequest(new { result.Message });
            }

            if (result.Entity == new UpdateAssetDto())
            {
                return NotFound();
            }
            else
            {
                return Ok(new
                {
                    Asset = result.Entity,
                });
            }
        }


        [ProducesResponseType(typeof(HandlerResult<AssetDto>), StatusCodes.Status200OK)]
        [Authorize(AuthenticationSchemes = JwtConfigConstants.AuthSchemes)]
        [HttpPut]
        [Route("Move")]
        public async Task<IActionResult> MoveAsset(MoveAssetsDto assets)
        {
            var userId = _userManager.GetUserId(User);
            var result = await _mediator.Send(new MoveAssetsRequest(assets, userId));

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
                Asset = result.Entity,
            });
        }

        [ProducesResponseType(typeof(HandlerResult<UpdateAssetStatusDto>), StatusCodes.Status200OK)]
        [Authorize(AuthenticationSchemes = JwtConfigConstants.AuthSchemes)]
        [HttpPut]
        [Route("Status")]
        public async Task<IActionResult> ArchiveAssets(UpdateAssetStatusDto assets)
        {
            var userId = _userManager.GetUserId(User);
            var result = await _mediator.Send(new UpdateAssetStatusRequest(assets, userId));

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

        [ProducesResponseType(typeof(HandlerResult<WopiParamDto>), StatusCodes.Status200OK)]
        [Authorize(AuthenticationSchemes = JwtConfigConstants.AuthSchemes)]
        [HttpGet]
        [Route("View/{id}")]
        public async Task<IActionResult> ViewDoc(string id)
        {
            var result = await _mediator.Send(new GetWopiParamsRequest(id, 1));

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
                Params = result.Entity,
            });
        }

        [ProducesResponseType(typeof(HandlerResult<WopiParamDto>), StatusCodes.Status200OK)]
        [Authorize(AuthenticationSchemes = JwtConfigConstants.AuthSchemes)]
        [HttpGet]
        [Route("Edit/{id}")]
        public async Task<IActionResult> EditDoc(string id)
        {
            var result = await _mediator.Send(new GetWopiParamsRequest(id, 2));

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
                Params = result.Entity,
            });
        }

        // ORIGINAL FILE DOWNLOAD
        [ProducesResponseType(typeof(HandlerResult<FileResult>), StatusCodes.Status200OK)]
        [HttpGet]
        [Route("Download/{assetKey}/{emailAddress}/{showWaterMark}")]
        public async Task<IActionResult> DownloadAsset(string assetKey, string emailAddress, bool showWaterMark)
        {
            var result = await _mediator.Send(new DownloadAssetRequest(assetKey, emailAddress, showWaterMark));

            if (result.ResultType == ResultType.NoData)
            {
                return NotFound(new { result.Message });
            }
            if (result.ResultType == ResultType.BadRequest || result.ResultType == ResultType.Fail)
            {
                return BadRequest(new { result.Message });
            }

            return File(result.Entity.Content, result.Entity.ContentType, result.Entity.FileName);
        }

        // FILE DOWNLOAD WITH CONVERSION
        [ProducesResponseType(typeof(HandlerResult<FileResult>), StatusCodes.Status200OK)]
        [HttpGet]
        [Route("Download/{assetKey}/{emailAddress}/{showWaterMark}/{extension}")]
        public async Task<IActionResult> DownloadAssetConvert(string assetKey, string emailAddress, bool showWaterMark, string extension)
        {
            var result = await _mediator.Send(new DownloadAssetRequest(assetKey, emailAddress, showWaterMark, extension));

            if (result.ResultType == ResultType.NoData)
            {
                return NotFound(new { result.Message });
            }
            if (result.ResultType == ResultType.BadRequest || result.ResultType == ResultType.Fail)
            {
                return BadRequest(new { result.Message });
            }

            return File(result.Entity.Content, result.Entity.ContentType, result.Entity.FileName);
        }

        // PDF DOWNLOAD CONVERT TO IMAGE
        [ProducesResponseType(typeof(HandlerResult<FileResult>), StatusCodes.Status200OK)]
        [HttpGet]
        [Route("DownloadPDFAsImage/{assetKey}/{emailAddress}/{showWaterMark}/{extension}")]
        public async Task<IActionResult> DownloadPDFAsImages(string assetKey, string emailAddress, bool showWaterMark, string extension)
        {
            var pages = await _mediator.Send(new DownloadPDFToImageRequest(assetKey, emailAddress, showWaterMark, extension));

            if (pages.Entity.Count == 0)
            {
                return BadRequest("No data");
            }

            var fileName = "MyZipfile.zip";
            var result = new HttpResponseMessage()
            {
                Content = new PushStreamContent(async (outputStream, httpContext, transportContext) =>
                {
                    using (var zipStream = new ZipOutputStream(outputStream))
                    {
                        foreach (var kvp in pages.Entity)
                        {
                            zipStream.PutNextEntry(kvp.Key);
                            using (var stream = kvp.Value)
                            {
                                stream.Seek(0, SeekOrigin.Begin);
                                await stream.CopyToAsync(zipStream);
                            }
                        }
                    }
                })
            };
            return new FileStreamResult(result.Content.ReadAsStreamAsync().Result, MediaTypeHeaderValue.Parse("application/octet-stream"))
            {
                FileDownloadName = fileName
            };
        }

        // OFFICE DOWNLOAD CONVERT TO PDF
        [ProducesResponseType(typeof(HandlerResult<FileResult>), StatusCodes.Status200OK)]
        [HttpGet]
        [Route("DownloadOfficeAsPdf/{assetKey}/{emailAddress}")]
        public async Task<IActionResult> DownloadOfficeAsPdf(string assetKey, string emailAddress)
        {
            var result = await _mediator.Send(new DownloadOfficeToPDFRequest(assetKey, emailAddress));
            
            if (result.Entity != null)
            {
                return new FileStreamResult(result.Entity.Content.ReadAsStreamAsync().Result, MediaTypeHeaderValue.Parse("application/octet-stream"));
            } 
            else
            {
                return NotFound();
            }
        }

        [ProducesResponseType(typeof(HandlerResult<FileResult>), StatusCodes.Status200OK)]
        [HttpGet]
        [Route("Download/Package/{assetKey}")]
        public async Task<IActionResult> DownloadAssetPackage(string assetKey)
        {
            var userId = _userManager.GetUserId(User);
            var result = await _mediator.Send(new DownloadAssetPackageRequest(assetKey, userId, false));

            if (result.ResultType == ResultType.NoData)
            {
                return NotFound(new { result.Message });
            }
            if (result.ResultType == ResultType.BadRequest || result.ResultType == ResultType.Fail)
            {
                return BadRequest(new { result.Message });
            }

            return File(result.Entity.Content, result.Entity.ContentType, result.Entity.FileName);
        }

        [ProducesResponseType(typeof(HandlerResult<ShareDto>), StatusCodes.Status200OK)]
        [Authorize(AuthenticationSchemes = JwtConfigConstants.AuthSchemes)]
        [HttpGet]
        [Route("Share/{assetKey}/{emailAddress}")]
        public async Task<IActionResult> ShareAsset(string assetKey, string emailAddress)
        {
            var shareDto = new ShareDto()
            {
                AssetKey = assetKey,
                EmailAddress = emailAddress
            };

            var result = await _mediator.Send(new ShareAssetRequest(shareDto));

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
                Params = result.Entity,
            });
        }

        [ProducesResponseType(typeof(HandlerResult<UploadToDynamicsDto>), StatusCodes.Status200OK)]
        [HttpPut]
        [Route("UploadToDynamics")]
        public async Task<IActionResult> UploadToDynamics(List<UploadToDynamicsDto> uploadToDynamicsDTOs)
        {
            var result = await _mediator.Send(new UploadToDynamicsRequest(uploadToDynamicsDTOs));

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
                Params = result.Entity,
            });
        }


        [ProducesResponseType(typeof(HandlerResult<List<AssetDto>>), StatusCodes.Status200OK)]
        [Authorize(AuthenticationSchemes = JwtConfigConstants.AuthSchemes)]
        [HttpGet]
        [Route("Approved")]
        public async Task<IActionResult> GetApprovedAssets()
        {
            var result = await _mediator.Send(new GetAssetsByStatusRequest((int)AssetStatus.Approved));

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

        [ProducesResponseType(typeof(HandlerResult<List<AssetDto>>), StatusCodes.Status200OK)]
        [HttpGet]
        [Route("AssetTrend")]
        public IActionResult GetAssetTrend()
        {

            var assets = (from a in _dbcontext.Assets
                          join av in _dbcontext.AssetVersions
                          on a.Id equals av.AssetId
                          where av.ActiveVersion == 1
                          select new
                          {
                              a.Id,
                              a.Title,
                              av.FileName,
                              a.FolderId,
                              av.DownloadCount,
                          }
                          ).ToList();
            var result = new List<AssetBIDto>();
            var alldownloadHistory = _dbcontext.AssetAudit.Where(t => t.AuditType == 7);//Asset Downloaded
            var allfolders = _dbcontext.Folders;
            //var allcompanies = _dbcontext.Companies;
            foreach (var item in assets)
            {
                var downLoadList = alldownloadHistory.Where(t => t.AssetId == item.Id);
                var timeTracking = new List<DateTimeOffset>();
                if (downLoadList != null)
                {
                    foreach (var t in downLoadList)
                    {
                        timeTracking.Add(t.AuditCreatedDate);
                    }
                }
                var folder = allfolders.Where(f => f.Id == item.FolderId).FirstOrDefault();
                //var rootId = 0;
                //var company = new Company();
                //if(folder != null && folder.ParentFolderId != null) 
                //{
                //    var pid = folder.ParentFolderId;
                //    rootId = folder.Id;
                //    while (pid != null)
                //    {
                //        var pfolder = allfolders.Where(f => f.Id == pid).FirstOrDefault();
                //        pid = pfolder.ParentFolderId;
                //        if(pid != null)
                //        { 
                //            rootId = (int)pfolder.Id; 
                //        }

                //    }
                //}
                //if (rootId > 0)
                //{
                //    company = allcompanies.Where(c => c.RootFolderId == rootId).FirstOrDefault();                    
                //}
                AssetBIDto asset = new AssetBIDto
                {
                    Id = item.Id,
                    Title = item.Title,
                    FileName = item.FileName,
                    FolderId = item.FolderId,
                    DownloadCount = item.DownloadCount,
                    AuditTracking = timeTracking,
                    CompanyId = item.FolderId,
                    CompanyName = folder.FolderName
                };
                result.Add(asset);
            };
            return Ok(result);
        }

        [ProducesResponseType(typeof(HandlerResult<AssetDto>), StatusCodes.Status200OK)]
        [Authorize(AuthenticationSchemes = JwtConfigConstants.AuthSchemes)]
        [HttpGet]
        [Route("UpdateAssetsUrl/{key}")]
        public async Task<IActionResult> UpdateAssetsUrl(string key)
        {
            if (key == "updateSimple5")
            {
                var result = await _mediator.Send(new UpdateAssetsUrlRequest());
            }
            return Ok("Update done");
        }

        [ProducesResponseType(typeof(HttpResponseMessage), StatusCodes.Status200OK)]
        [HttpGet]
        [Route("DownloadBulk/{assetsIds}/{emailAddress}/{showWatermark}")]
        public async Task<IActionResult> DownloadBulkAsset(string assetsIds, string emailAddress, bool showWatermark)
        {
            if (string.IsNullOrEmpty(assetsIds))
            {
                return BadRequest("No ids");
            }

            var assets = await _mediator.Send(new DownloadBulkAssetsRequest(assetsIds, emailAddress, showWatermark));

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

        [ProducesResponseType(typeof(HttpResponseMessage), StatusCodes.Status200OK)]
        [Authorize(AuthenticationSchemes = JwtConfigConstants.AuthSchemes)]
        [HttpGet]
        [Route("ShareBulk/{assetsIds}/{folderIds}")]
        public async Task<IActionResult> ShareBulkAsset(string assetsIds, string folderIds)
        {
            if (string.IsNullOrEmpty(assetsIds) || string.IsNullOrEmpty(folderIds))
            {
                return BadRequest("No ids");
            }

            var result = await _mediator.Send(new ShareBulkAssetRequest(assetsIds, folderIds));
            return Ok(result);
        }

        [ProducesResponseType(typeof(HttpResponseMessage), StatusCodes.Status200OK)]
        [Authorize(AuthenticationSchemes = JwtConfigConstants.AuthSchemes)]
        [HttpGet]
        [Route("Test/{type}")]
        public async Task<IActionResult> LoadingTimeTest(string type)
        {
            var result = await _mediator.Send(new GetAssetTestRequest(type));
            return Ok(result);
        }

        [ProducesResponseType(typeof(HttpResponseMessage), StatusCodes.Status200OK)]
        [HttpGet]
        [Route("ArchiveClean/{days}")]
        public async Task<IActionResult> ArchiveClean(int days)
        {
            var temp = await _mediator.Send(new GetArchiveRequest(0, days));
            var userId = _userManager.GetUserId(User);
            var assetList = temp.Entity.ToList();
            var assets = new UpdateAssetStatusDto();
            assets.AssetIds = new List<int>();
            assets.Status = Convert.ToInt32(AssetStatus.Deleted);
            assetList.ForEach(a =>
            {
                if (a.Id.HasValue)
                {
                    assets.AssetIds.Add((int)a.Id);
                };
            });
            var result = await _mediator.Send(new UpdateAssetStatusRequest(assets, userId));

            if (result.ResultType == ResultType.NoData)
            {
                return NotFound(new { result.Message });
            }
            if (result.ResultType == ResultType.BadRequest || result.ResultType == ResultType.Fail)
            {
                return BadRequest(new { result.Message });
            }

            return Ok(result.Entity);
        }

        [ProducesResponseType(typeof(HttpResponseMessage), StatusCodes.Status200OK)]
        [HttpGet]
        [Route("crc")]
        public async Task<IActionResult> VersionsCRC(string type)
        {
            var result = await _mediator.Send(new GetAllAssetVersionsCRC32Request());
            return Ok(result);
        }

        [ProducesResponseType(typeof(HttpResponseMessage), StatusCodes.Status200OK)]
        [HttpPut]
        [Route("duplicateAsset")]
        public async Task<IActionResult> DuplicateAsset(AssetDto asset)
        {
            var result = await _mediator.Send(new GetDuplicateAssetRequest(asset));
            if (result.ResultType == ResultType.NoData)
            {
                return NotFound(new { result.Message });
            }
            return Ok(result.Entity);
        }

        [ProducesResponseType(typeof(HandlerResult<List<AssetDto>>), StatusCodes.Status200OK)]
        [Authorize(AuthenticationSchemes = JwtConfigConstants.AuthSchemes)]
        [HttpPost]
        [Route("copyToFolder")]
        public async Task<IActionResult> CopyToFolder(CopyToFolderDto copyInput)
        {
            var userId = _userManager.GetUserId(User);
            var result = await _mediator.Send(new DuplicateAssetsRequest(copyInput.AssetIds, copyInput.FolderId, userId));

            if (result.ResultType == ResultType.NoData)
            {
                return NotFound(new { result.Message });
            }
            if (result.ResultType == ResultType.BadRequest || result.ResultType == ResultType.Fail)
            {
                return BadRequest(new { result.Message });
            }
            return Ok();
        }

        [ProducesResponseType(typeof(HandlerResult<List<AssetDto>>), StatusCodes.Status200OK)]
        [Authorize(AuthenticationSchemes = JwtConfigConstants.AuthSchemes)]
        [HttpPost]
        [Route("bulkUpdateTags")]
        public async Task<IActionResult> BulkUpdateAssetTags(UpdateAssetTagsDto input)
        {
            var userId = _userManager.GetUserId(User);
            var result = await _mediator.Send(new UpdateAssetTagsRequest(input.AssetsToUpdate, userId));

            if (result.ResultType == ResultType.NoData)
            {
                return NotFound(new { result.Message });
            }
            if (result.ResultType == ResultType.BadRequest || result.ResultType == ResultType.Fail)
            {
                return BadRequest(new { result.Message });
            }
            return Ok();
        }

        [ProducesResponseType(typeof(HandlerResult<string>), StatusCodes.Status200OK)]
        [Authorize(AuthenticationSchemes = JwtConfigConstants.AuthSchemes)]
        [HttpPost]
        [Route("uploadChunks")]
        public async Task<IActionResult> UploadChunks(string id, string fileName)
        {
            var _responseData = new HandlerResult<string>();
            try
            {
                var chunkNumber = id;
                string newpath = Path.Combine(_configuration["TargetFolder"] + "/Temp", fileName + chunkNumber);
                System.IO.Directory.CreateDirectory(_configuration["TargetFolder"] + "/Temp");//if there is no folder, create it
                using (FileStream fs = System.IO.File.Create(newpath))
                {
                    byte[] bytes = new byte[1048576 * Convert.ToInt32(_configuration["ChunkSize"])];
                    int bytesRead = 0;
                    while ((bytesRead = await Request.Body.ReadAsync(bytes, 0, bytes.Length)) > 0)
                    {
                        fs.Write(bytes, 0, bytesRead);
                    }
                }
                _responseData.Entity = "success";
            }
            catch (Exception ex)
            {
                _responseData.Message = ex.Message;
                _responseData.ResultType = ResultType.Fail;
                return BadRequest(new { _responseData.Message });
            }
            return Ok(_responseData);
        }

        [ProducesResponseType(typeof(HandlerResult<string>), StatusCodes.Status200OK)]
        [Authorize(AuthenticationSchemes = JwtConfigConstants.AuthSchemes)]
        [HttpPost]
        [Route("uploadComplete")]
        public async Task<IActionResult> UploadComplete(string fileName, string fileOriginalName, string fileType, int folderId, int projectId)
        {
            var _responseData = new HandlerResult<string>();
            try
            {
                string tempPath = _configuration["TargetFolder"] + "/Temp";
                string newOriginalName = DateTime.Now.Millisecond.ToString() + fileOriginalName;
                string newPath = Path.Combine(tempPath, newOriginalName);
                string[] filePaths = Directory.GetFiles(tempPath).Where(p => p.Contains(fileName)).OrderBy(p => Int32.Parse(p.Replace(fileName, "$").Split('$')[1])).ToArray();
                foreach (string filePath in filePaths)
                {
                    MergeChunks(newPath, filePath);
                }

                var asset = new AssetDto();
                using (FileStream fs = new FileStream(newPath, FileMode.Open, FileAccess.Read))
                {
                    byte[] byteArray = new byte[fs.Length];
                    fs.Read(byteArray, 0, byteArray.Length);
                    asset.FileBytes = byteArray;
                }
                asset.Name = fileOriginalName;
                asset.FileName = fileOriginalName;
                asset.Extension = fileOriginalName.Split('.').Last();
                asset.FileType = String.IsNullOrWhiteSpace(fileType) ? "" : fileType;
                asset.FolderId = folderId;
                asset.ProjectId = projectId;
                var userId = _userManager.GetUserId(User);
                var result = await _mediator.Send(new AddAssetRequest(asset, userId));

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
                    Asset = result.Entity
                });
                //System.IO.File.Move(Path.Combine(tempPath, newOriginalName), Path.Combine(_configuration["TargetFolder"], newOriginalName));
            }
            catch (Exception ex)
            {
                _responseData.Message = ex.Message;
                _responseData.ResultType = ResultType.Fail;
                return BadRequest(new { _responseData.Message });
            }
            return Ok(_responseData);
        }

        private static void MergeChunks(string chunk1, string chunk2)
        {
            FileStream fs1 = null;
            FileStream fs2 = null;
            try
            {
                fs1 = System.IO.File.Open(chunk1, FileMode.Append);
                fs2 = System.IO.File.Open(chunk2, FileMode.Open);
                byte[] fs2Content = new byte[fs2.Length];
                fs2.Read(fs2Content, 0, (int)fs2.Length);
                fs1.Write(fs2Content, 0, (int)fs2.Length);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message + " : " + ex.StackTrace);
            }
            finally
            {
                if (fs1 != null) fs1.Close();
                if (fs2 != null) fs2.Close();
                System.IO.File.Delete(chunk2);
            }
        }

        [ProducesResponseType(typeof(HandlerResult<string>), StatusCodes.Status200OK)]
        [Authorize(AuthenticationSchemes = JwtConfigConstants.AuthSchemes)]
        [HttpPost]
        [Route("addThumbnail")]
        public async Task<IActionResult> AddThumbnail(AssetThumbnailDto thumbnailDto)
        {
            var userId = _userManager.GetUserId(User);
            var result = await _mediator.Send(new AddAssetThumbnailRequest(thumbnailDto, userId));

            if (result.ResultType == ResultType.NoData || thumbnailDto.AssetId ==0)
            {
                return NotFound(new { result.Message });
            }
            if (result.ResultType == ResultType.BadRequest || result.ResultType == ResultType.Fail)
            {
                return BadRequest(new { result.Message });
            }
            return Ok();
        }

        [ProducesResponseType(typeof(HandlerResult<string>), StatusCodes.Status200OK)]
        [Authorize(AuthenticationSchemes = JwtConfigConstants.AuthSchemes)]
        [HttpPost]
        [Route("removeThumbnail")]
        public async Task<IActionResult> RemoveThumbnail(AssetThumbnailDto thumbnailDto)
        {
            var userId = _userManager.GetUserId(User);
            var result = await _mediator.Send(new RemoveAssetThumbnailRequest(thumbnailDto, userId));

            if (result.ResultType == ResultType.NoData || thumbnailDto.AssetId == 0)
            {
                return NotFound(new { result.Message });
            }
            if (result.ResultType == ResultType.BadRequest || result.ResultType == ResultType.Fail)
            {
                return BadRequest(new { result.Message });
            }
            return Ok();
        }

        [ProducesResponseType(typeof(HandlerResult<string>), StatusCodes.Status200OK)]
        [Authorize(AuthenticationSchemes = JwtConfigConstants.AuthSchemes)]
        [HttpPost]
        [Route("addPackage")]
        public async Task<IActionResult> AddPackage(AssetPackageDto packageDto)
        {
            var userId = _userManager.GetUserId(User);
            var result = await _mediator.Send(new AddAssetPackageRequest(packageDto, userId));

            if (result.ResultType == ResultType.NoData || packageDto.AssetId == 0)
            {
                return NotFound(new { result.Message });
            }
            if (result.ResultType == ResultType.BadRequest || result.ResultType == ResultType.Fail)
            {
                return BadRequest(new { result.Message });
            }
            return Ok();
        }
        [ProducesResponseType(typeof(HandlerResult<string>), StatusCodes.Status200OK)]
        [Authorize(AuthenticationSchemes = JwtConfigConstants.AuthSchemes)]
        [HttpPost]
        [Route("convertPDFPreview")]
        public async Task<IActionResult> ConvertPDFPreview(AssetThumbnailDto thumbnailDto)
        {
            var userId = _userManager.GetUserId(User);
            var result = await _mediator.Send(new ConvertPDFToThumbnailImageRequest(thumbnailDto, userId));

            if (result.ResultType == ResultType.NoData)
            {
                return NotFound(new { result.Message });
            }
            if (result.ResultType == ResultType.BadRequest || result.ResultType == ResultType.Fail)
            {
                return BadRequest(new { result.Message });
            }
            byte[] bytes = new byte[result.Entity.Content.Length];
            result.Entity.Content.Read(bytes, 0, bytes.Length);
            // 设置当前流的位置为流的开始
            result.Entity.Content.Seek(0, SeekOrigin.Begin);
            return Ok(bytes);
        }

    }
}