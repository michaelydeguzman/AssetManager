using AutoMapper;
using Azure.Storage.Blobs.Models;
using DAM.Application;
using DAM.Application.Assets.Dtos;
using DAM.Application.Services.Interfaces;
using DAM.Application.Users.Constants;
using DAM.Application.Users.Dtos;
using DAM.Application.Users.Models;
using DAM.Domain.Entities;
using DAM.Domain.Entities.Identity;
using DAM.Persistence;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace DAM.Web.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AppController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private IEmailService _emailService;
        private IConfiguration _configuration;
        private readonly IMapper _mapper;
        private readonly IDbContext _dbcontext;
        private readonly IAzureStorageService _azureStorageService;
        private string _baseUrl;
        public AppController(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager, IDbContext dbcontext, IEmailService emailService, IConfiguration configuration, IMapper mapper, IAzureStorageService azureStorageService)
        {
            _userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
            _signInManager = signInManager ?? throw new ArgumentNullException(nameof(signInManager));
            _emailService = emailService ?? throw new ArgumentNullException(nameof(emailService));
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
            _dbcontext = dbcontext ?? throw new ArgumentNullException(nameof(dbcontext));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _azureStorageService = azureStorageService ?? throw new ArgumentNullException(nameof(azureStorageService));
            _baseUrl = _configuration["BaseUrl"];
        }

        // ASP .NET CORE Identity Implementation
        [HttpGet]
        [Authorize(AuthenticationSchemes = JwtConfigConstants.AuthSchemes)]
        [Route("Users")]
        public async Task<IActionResult> GetAppUsers()
        {
            var users = new List<ApplicationUser>();
            users = _userManager.Users.ToList();
            var allUserRole = _dbcontext.UserRoles.ToList();
            var allUserFolders = _dbcontext.UserFolders.ToList();
            var allFolders = _dbcontext.Folders.ToList();
            var allCompanies = _dbcontext.Companies.ToList();

            var result = _mapper.Map<IEnumerable<ApplicationUserDto>>(users);
            foreach (var user in result)
            {
                if (!string.IsNullOrEmpty(user.ImageFileExtension))
                {
                    user.ImageUrl = _azureStorageService.GetBlobSasUri(_configuration, string.Concat(user.Id, ".", user.ImageFileExtension));
                }
                user.UserRole = allUserRole.Where(ur => ur.Id == user.UserRoleId).FirstOrDefault();
                user.UserFolders = allUserFolders.Where(uf => uf.UserId == user.Id).ToList();
                if (user.UserFolders != null && user.UserFolders.Count > 0)
                {
                    foreach (var folder in user.UserFolders)
                    {
                        folder.Folder = allFolders
                            .Where(f => f.Id == folder.FolderId)
                            .Select(f => new Folder
                            {
                                Id = f.Id,
                                FolderName = f.FolderName,
                                Description = f.Description,
                                ParentFolderId = f.ParentFolderId,
                                Deleted = f.Deleted
                            })
                            .FirstOrDefault();
                    }
                }

                user.CompanyName = user.CompanyId.HasValue ? allCompanies.FirstOrDefault(x => x.Id == user.CompanyId.Value).CompanyName : _dbcontext.Folders.FirstOrDefault(f => f.Id == 1).FolderName;
            }

            return Ok(new
            {
                Users = result,
            });
        }

        [HttpGet]
        [Authorize(AuthenticationSchemes = JwtConfigConstants.AuthSchemes)]
        [Route("Users/Check")]
        public IActionResult CheckLogin()
        {
            var userId = _userManager.GetUserId(User);
            if(userId == null)
            {
                return Unauthorized();
            }
            return Ok();
        }

        [HttpGet]
        [Authorize(AuthenticationSchemes = JwtConfigConstants.AuthSchemes)]
        [Route("Users/{id}")]
        public async Task<IActionResult> GetAppUsers(string id)
        {
            var users = new List<ApplicationUser>();
            ApplicationUserDto userResponse = new ApplicationUserDto();
            if (!string.IsNullOrEmpty(id))
            {
                var user = await _userManager.FindByIdAsync(id);

                userResponse = _mapper.Map<ApplicationUserDto>(user);
                userResponse.UserRole = _dbcontext.UserRoles.Where(ur => ur.Id == user.UserRoleId).FirstOrDefault();
                userResponse.UserFolders = _dbcontext.UserFolders.Where(uf => uf.UserId == user.Id).ToList();
                if (!string.IsNullOrEmpty(user.ImageFileExtension))
                {
                    userResponse.ImageUrl = _azureStorageService.GetBlobSasUri(_configuration, string.Concat(user.Id, ".", user.ImageFileExtension));
                }
                if (userResponse.UserFolders != null && userResponse.UserFolders.Count > 0)
                {
                    foreach (var folder in userResponse.UserFolders)
                    {
                        folder.Folder = _dbcontext.Folders
                            .Where(f => f.Id == folder.FolderId)
                            .Select(f => new Folder
                            {
                                Id = f.Id,
                                FolderName = f.FolderName,
                                Description = f.Description,
                                ParentFolderId = f.ParentFolderId,
                                Deleted = f.Deleted
                            })
                            .FirstOrDefault();
                    }
                }
            }

            return Ok(new
            {
                Users = userResponse,
            });
        }

        [HttpGet]
        [Authorize(AuthenticationSchemes = JwtConfigConstants.AuthSchemes)]
        [Route("Users/Company/{id}")]
        public async Task<IActionResult> GetCompanyUsers(int id)
        {

            var userId = _userManager.GetUserId(User);

            var user = await _userManager.FindByIdAsync(userId);

            if (user == null)
            {
                return Unauthorized();
            }

            if (user.UserRoleId != (int)UserRoleEnum.COMPANY_ADMIN && user.UserRoleId != (int)UserRoleEnum.DAM_ADMIN)
            {
                return Unauthorized();
            }

            var users = new List<ApplicationUser>();
            ApplicationUserDto userResponse = new ApplicationUserDto();

            var companyUsers = _userManager.Users.Where(x => x.CompanyId == id).ToList();

            var result = _mapper.Map<IEnumerable<ApplicationUserDto>>(companyUsers);
            foreach (var companyUser in result)
            {
                if (!string.IsNullOrEmpty(companyUser.ImageFileExtension))
                {
                    companyUser.ImageUrl = _azureStorageService.GetBlobSasUri(_configuration, string.Concat(companyUser.Id, ".", companyUser.ImageFileExtension));
                }
                companyUser.UserRole = _dbcontext.UserRoles.Where(ur => ur.Id == companyUser.UserRoleId).FirstOrDefault();
                companyUser.UserFolders = _dbcontext.UserFolders.Where(uf => uf.UserId == companyUser.Id).ToList();
                if (companyUser.UserFolders != null && companyUser.UserFolders.Count > 0)
                {
                    foreach (var folder in companyUser.UserFolders)
                    {
                        folder.Folder = _dbcontext.Folders
                            .Where(f => f.Id == folder.FolderId)
                            .Select(f => new Folder
                            {
                                Id = f.Id,
                                FolderName = f.FolderName,
                                Description = f.Description,
                                ParentFolderId = f.ParentFolderId,
                                Deleted = f.Deleted
                            })
                            .FirstOrDefault();
                    }
                }

                companyUser.CompanyName = companyUser.CompanyId.HasValue ? _dbcontext.Companies.FirstOrDefault(x => x.Id == companyUser.CompanyId.Value).CompanyName : "";
            }

            return Ok(new
            {
                Users = result,
            });
        }

        [HttpGet]
        [Route("Users/info")]
        public async Task<IActionResult> GetAppUsersTrends()
        {
            var users = new List<ApplicationUser>();
            users = _userManager.Users.ToList();
            foreach (var user in users)
            {
                user.UserRole = _dbcontext.UserRoles.Where(ur => ur.Id == user.UserRoleId).FirstOrDefault();
                user.Company = _dbcontext.Companies.Where(ur => ur.Id == user.CompanyId).FirstOrDefault();
            }

            return Ok(new
            {
                Users = users,
            });
        }

        [HttpPost]
        [Route("Users/Register")]
        public async Task<IActionResult> RegisterUser(SignUpDto signUpDetails)
        {
            var user = new ApplicationUser()
            {
                UserName = signUpDetails.FirstName + " " + signUpDetails.LastName,
                Email = signUpDetails.Email,
                UserRoleId = 1,
                Active = true
            };

            var result = await _userManager.CreateAsync(user, signUpDetails.Password);

            if (result.Succeeded)
            {
                var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);

                var confirmationLink = Url.Action("ConfirmEmail", "App", new { userId = user.Id, token = token }, Request.Scheme);

                // Send email
                //_emailService.SendEmail(_configuration, AppMessageConstants.RegistrationMessage, user.Email, confirmationLink);

                return Ok(new
                {
                    User = user,
                    ConfirmationLink = confirmationLink // To comment out
                });
            }
            else
            {
                return BadRequest(new
                {
                    Message = AppMessageConstants.RegistrationFailed + " " + result.Errors.First().Description
                });
            }
        }

        [HttpPost]
        [Route("Users/Add")]
        public async Task<IActionResult> AddNewUser(AddUserDto signUpDetails)
        {
            var user = new ApplicationUser()
            {
                UserName = signUpDetails.FirstName + " " + signUpDetails.LastName,
                Email = signUpDetails.Email,
                UserRoleId = signUpDetails.RoleId,
                Active = true
            };

            var result = await _userManager.CreateAsync(user);

            if (result.Succeeded)
            {
                // call 
                var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);

                var confirmationLink = Url.Action("Users/ConfirmEmail", "App", new { userId = user.Id, token = token }, Request.Scheme);

                // Send email
                _emailService.SendEmail(_configuration, AppMessageConstants.RegistrationMessage, user.Email, confirmationLink);

                return Ok(new
                {
                    User = user,
                    ConfirmationLink = confirmationLink // To comment out
                });
            }
            else
            {
                return BadRequest(new
                {
                    Message = AppMessageConstants.EmailConfirmationFailed
                });
            }
        }

        [HttpPost]
        [Route("Users/AddPassword")]
        public async Task<IActionResult> AddPassword(ResetPasswordDto resetPasswordDetails)
        {
            var user = await _userManager.FindByEmailAsync(resetPasswordDetails.Email);

            if (user == null)
            {
                return NotFound(new
                {
                    Message = AppMessageConstants.AddPasswordFailed + " " + AppMessageConstants.InvalidEmail
                });
            }

            if (!string.IsNullOrEmpty(user.PasswordHash))
            {
                return BadRequest(new
                {
                    Message = AppMessageConstants.AddPasswordFailed + " " + AppMessageConstants.HasPassword
                });
            }

            var result = await _userManager.AddPasswordAsync(user, resetPasswordDetails.Password);

            if (result.Succeeded)
            {
                return Ok();
            }
            else
            {
                return BadRequest(new
                {
                    Message = AppMessageConstants.AddPasswordFailed + " " + result.Errors.First().Description
                });
            }
        }

        [HttpPost]
        [Route("Users/ChangePassword")]
        public async Task<IActionResult> ChangePassword(ResetPasswordDto resetPasswordDetails)
        {
            var userId = _userManager.GetUserId(User);

            var user = await _userManager.FindByIdAsync(userId);

            if (user == null)
            {
                return NotFound(new
                {
                    Message = AppMessageConstants.InvalidInputs
                });
            }

            user.UserName = resetPasswordDetails.DisplayName;
            var result = await _userManager.UpdateAsync(user);
            result = await _userManager.ChangePasswordAsync(user, resetPasswordDetails.OldPassword, resetPasswordDetails.Password);

            if (result.Succeeded)
            {
                return Ok();
            }
            else
            {
                return BadRequest(new
                {
                    Message = AppMessageConstants.InvalidInputs + " " + result.Errors.First().Description
                });
            }
        }

        [HttpPost]
        [Route("Users/ResendVerification")]
        public async Task<IActionResult> ResendVerificationEmail(SignInDto userDto)
        {
            try
            {
                var user = await _userManager.FindByEmailAsync(userDto.Email);

                if (user != null)
                {
                    if (!user.EmailConfirmed)
                    {
                        // Send email
                        var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);

                        var confirmationLink = Url.Action("ConfirmEmail", "Users", new { userId = user.Id, token = token, userEmail = user.Email }, Request.Scheme);

                        _emailService.SaveEmailVerificationToEmailQueue(user, user, confirmationLink);
                    }
                    else
                    {
                        return BadRequest(new
                        {
                            Message = AppMessageConstants.EmailAlreadyConfirmed
                        });
                    }
                }
                else
                {
                    return NotFound(new
                    {
                        Message = AppMessageConstants.InvalidEmail
                    });
                }
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    Message = AppMessageConstants.EmailConfirmationFailed + ex.Message
                });
            }

            return Ok();
        }

        [HttpPost]
        [Route("Users/UploadProfilePicture")]
        public async Task<IActionResult> UploadProfilePicture(ProfilePictureDto profilePictureDto)
        {
            var userId = _userManager.GetUserId(User);

            var user = await _userManager.FindByIdAsync(userId);

            if (user == null)
            {
                return NotFound(new
                {
                    Message = AppMessageConstants.InvalidInputs
                });
            }

            using (Stream stream = new MemoryStream(profilePictureDto.FileBytes))
            {
                BlobProperties userProps = _azureStorageService.UploadFileToStorage(_configuration, stream, string.Concat(user.Id, ".", profilePictureDto.ImageFileExtension), out Uri assetUri, false, true);
                user.ImageFileExtension = profilePictureDto.ImageFileExtension;
            }

            var result = await _userManager.UpdateAsync(user);
            if (result.Succeeded)
            {
                profilePictureDto.ImageUrl = _azureStorageService.GetBlobSasUri(_configuration, string.Concat(user.Id, ".", profilePictureDto.ImageFileExtension));
                return Ok(new
                {
                    Response = profilePictureDto
                }
                    );
            }
            else
            {
                return BadRequest(new
                {
                    Message = AppMessageConstants.InvalidInputs + " " + result.Errors.First().Description
                });
            }
        }


        private static List<FileInfo> files = new List<FileInfo>();  // List that will hold the files and subfiles in path
        private static List<DirectoryInfo> folders = new List<DirectoryInfo>(); // List that hold direcotries that cannot be accessed

        [HttpPost]
        [Route("Migrate/Folders")]
        public async Task<IActionResult> MigrateFolders() //DONT USE THIS FOR PARTNERS
        {
            DirectoryInfo di = new DirectoryInfo("D:\\Dubber-File-Structure\\Marketing");//change this path to the folder name
            Folder newFolder;
            newFolder = new Folder
            {
                FolderName = "Marketing",//change this foldername
                Description = "Marketing",//change this foldername
                ParentFolderId = 1,
                Deleted = false
            };

            //_dbcontext.Folders.Add(newFolder);
            //_dbcontext.SaveChanges();

            FullDirList(di, "*", newFolder.Id);

            return Ok();
        }

        private void FullDirList(DirectoryInfo dir, string searchPattern, int? parentFolderId = 0)
        {
            foreach (DirectoryInfo d in dir.GetDirectories())
            {
                Folder newFolder;
                var folderDir = d.Name.Split("\\");
                var folderName = folderDir[folderDir.Length - 1];
                newFolder = new Folder
                {
                        
                    FolderName = folderName,
                    Description = folderName,
                    ParentFolderId = parentFolderId,
                    Deleted = false
                };

                //_dbcontext.Folders.Add(newFolder);
                //_dbcontext.SaveChanges();

                FullDirList(d, searchPattern, newFolder.Id);
            }
        }

        [HttpPost]
        [Route("Users/ForgotPassword")]
        public async Task<IActionResult> ForgotPassword(ResetPasswordDto resetPasswordDetails)
        {
            var user = await _userManager.FindByEmailAsync(resetPasswordDetails.Email);

            if (user == null)
            {
                return NotFound(new
                {
                    Message = AppMessageConstants.ResetPasswordFailed + " " + AppMessageConstants.InvalidEmail
                });
            }

            var token = await _userManager.GeneratePasswordResetTokenAsync(user);

            // Send email

            _emailService.SaveForgotPasswordToEmailQueue(user, token);

            return Ok();
        }

        [HttpPost]
        [Route("Users/ResetPassword")]
        public async Task<IActionResult> ResetPassword(ResetPasswordDto resetPasswordDetails)
        {
            var user = await _userManager.FindByEmailAsync(resetPasswordDetails.Email);

            if (user == null)
            {
                return NotFound(new
                {
                    Message = AppMessageConstants.ResetPasswordFailed + " " + AppMessageConstants.InvalidEmail
                });
            }

            var result = await _userManager.ResetPasswordAsync(user, resetPasswordDetails.Token, resetPasswordDetails.Password);

            if (result.Succeeded)
            {
                return Ok();
            }
            else
            {
                return BadRequest(new
                {
                    Message = AppMessageConstants.ResetPasswordFailed + " " + result.Errors.First().Description
                });
            }
        }

        [HttpGet]
        [Route("Users/ConfirmEmail")]
        public async Task<IActionResult> ConfirmEmail(string userId, string token)
        {
            if (userId == null || token == null)
            {
                return BadRequest(new
                {
                    Message = AppMessageConstants.EmailConfirmationFailed + " " + AppMessageConstants.InvalidInputs
                }); ;
            }

            var user = await _userManager.FindByIdAsync(userId);

            if (user == null)
            {
                return NotFound(new
                {
                    Message = AppMessageConstants.EmailConfirmationFailed + " " + AppMessageConstants.InvalidEmail
                });
            }

            var result = await _userManager.ConfirmEmailAsync(user, token);

            if (result.Succeeded)
            {
                return Ok();
            }
            else
            {
                return BadRequest(new
                {
                    Message = AppMessageConstants.EmailConfirmationFailed + " " + result.Errors.First().Description
                });
            }
        }

        [HttpPost]
        [Route("Login")]
        [AllowAnonymous]
        public async Task<IActionResult> Login(SignInDto loginDetails)
        {
            if (loginDetails.IsDynamics)
            {
                loginDetails.Email = "simple.admin@simple.io";
                loginDetails.Password = "Simple5%";
            }

            var user = await _userManager.FindByEmailAsync(loginDetails.Email);

            if (user == null)
            {
                return NotFound(new
                {
                    Message = AppMessageConstants.InvalidEmail
                });
            }

            if (!(await _userManager.IsEmailConfirmedAsync(user)))
            {
                return BadRequest(new
                {
                    Message = AppMessageConstants.EmailUnconfirmed
                });
            }

            if (!user.Active)
            {
                return BadRequest(new
                {
                    Message = AppMessageConstants.DeactivatedAccount
                });
            }

            var result = await _signInManager.PasswordSignInAsync(user.UserName, loginDetails.Password, loginDetails.RememberMe, false);

            if (result.Succeeded)
            {
                // create Token
                var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(JwtConfigConstants.Key));
                var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

                var claims = new[]
                {
                    new Claim(JwtRegisteredClaimNames.Sub, user.Id),
                    new Claim(JwtRegisteredClaimNames.Email, user.Email),
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
                };

                var token = new JwtSecurityToken(
                    JwtConfigConstants.Issuer,
                    JwtConfigConstants.Audience,
                    claims,
                    expires: DateTime.UtcNow.AddDays(3),
                    signingCredentials: credentials
                );

                return Ok(new
                {
                    token = new JwtSecurityTokenHandler().WriteToken(token),
                    expiration = token.ValidTo,
                    user = new
                    {
                        userName = user.UserName,
                        id = user.Id,
                        roleId = user.UserRoleId,
                        email = user.Email
                    }
                });
            }
            else
            {
                return BadRequest(new
                {
                    Message = AppMessageConstants.InvalidPassword
                });
            }
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

        [ProducesResponseType(typeof(HandlerResult<String>), StatusCodes.Status200OK)]
        [Authorize(AuthenticationSchemes = JwtConfigConstants.AuthSchemes)]
        [HttpGet("PowerBIUrl")]
        public async Task<IActionResult> GetPowerBIUrl()
        {
          return Ok(_configuration["PowerBIUrl"]);
        }

        [ProducesResponseType(typeof(HandlerResult<String>), StatusCodes.Status200OK)]
        [Authorize(AuthenticationSchemes = JwtConfigConstants.AuthSchemes)]
        [HttpGet("AssetContainer")]
        public IActionResult GetAssetContainer()
        {
            return Ok(_configuration["AssetContainer"]);
        }

        [ProducesResponseType(typeof(HandlerResult<String>), StatusCodes.Status200OK)]
        [Authorize(AuthenticationSchemes = JwtConfigConstants.AuthSchemes)]
        [HttpGet("AssetPreviewContainer")]
        public IActionResult GetAssetPreviewContainer()
        {
            return Ok(_configuration["AssetPreviewContainer"]);
        }
    }
}