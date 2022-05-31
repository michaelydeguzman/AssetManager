using DAM.Application;
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
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static DAM.Application.Users.Requests.UserRequest;

namespace DAM.Web.Controllers
{
    [Route("admin/api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private IMediator _mediator;
        private readonly IDbContext _dbcontext;
        private readonly IEmailService _emailService;
        private IConfiguration _configuration;
        private readonly UserManager<ApplicationUser> _userManager;
        private string _baseUrl;

        public UsersController(IMediator mediator, UserManager<ApplicationUser> userManager, IDbContext dbcontext, IEmailService emailService, IConfiguration configuration)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
            _userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
            _dbcontext = dbcontext ?? throw new ArgumentNullException(nameof(dbcontext));
            _emailService = emailService ?? throw new ArgumentNullException(nameof(emailService));

            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
            _baseUrl = _configuration["BaseUrl"];
        }

        [ProducesResponseType(typeof(HandlerResult<List<UserDto>>), StatusCodes.Status200OK)]
        [Authorize(AuthenticationSchemes = JwtConfigConstants.AuthSchemes)]
        [HttpGet]
        [Route("{id}")]
        public async Task<IActionResult> GetUser(string id)
        {
            var result = await _mediator.Send(new GetUsersRequest(id));

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
                Users = result.Entity,
            });
        }

        [ProducesResponseType(typeof(HandlerResult<List<UserDto>>), StatusCodes.Status200OK)]
        [Authorize(AuthenticationSchemes = JwtConfigConstants.AuthSchemes)]
        [HttpGet]
        [Route("GetApprovers")]
        public async Task<IActionResult> GetApprovers()
        {
            var result = await _mediator.Send(new GetApproversRequest());

            var userId = _userManager.GetUserId(User);
            var currentUser = await _userManager.FindByIdAsync(userId);

            var users = new List<UserDto>();

            if(currentUser.CompanyId is null)
            {
                users = result.Entity.ToList();
            }
            else
            {
                foreach (UserDto u in result.Entity)
                {
                    if (u.CompanyId is null || u.CompanyId == currentUser.CompanyId)
                    {
                        users.Add(u);
                    }
                }
            }

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
                Users = users,
            });
        }

        [ProducesResponseType(typeof(HandlerResult<UserDto>), StatusCodes.Status201Created)]
        [Authorize(AuthenticationSchemes = JwtConfigConstants.AuthSchemes)]
        [HttpPost]
        [Route("Add")]
        public async Task<IActionResult> AddUser(UserDto user)
        {
            var result = await _mediator.Send(new AddUserRequest(user));

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
                User = result.Entity,
            });
        }

        [ProducesResponseType(typeof(HandlerResult<UserDto>), StatusCodes.Status200OK)]
        [Authorize(AuthenticationSchemes = JwtConfigConstants.AuthSchemes)]
        [HttpPut]
        [Route("UploadImage")]
        public async Task<IActionResult> UploadImage(UserDto user)
        {
            var result = await _mediator.Send(new AddUserImageRequest(user));

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
                User = result.Entity,
            });
        }

        [ProducesResponseType(typeof(HandlerResult<UserDto>), StatusCodes.Status200OK)]
        [HttpPut]
        [Route("InviteNewUser")]
        public async Task<IActionResult> InviteNewUser(InviteNewUserDto userDto)
        {
            var userId = _userManager.GetUserId(User);
            if(userId == null)
            {
                return Unauthorized();
            }
            var existingUser = _dbcontext.AppUsers.Where(u => u.Email == userDto.EmailAddress).FirstOrDefault();
            var inviter = _dbcontext.AppUsers.Where(u => u.Id == userId && u.EmailConfirmed).FirstOrDefault();
            inviter.UserRole = _dbcontext.UserRoles.Where(ur => ur.Id == inviter.UserRoleId).FirstOrDefault();

            if (existingUser == null && (inviter != null && inviter.UserRole.CanInvite))
            {
                var newUser = new ApplicationUser()
                {
                    UserName = userDto.DisplayName,
                    Email = userDto.EmailAddress,
                    UserRoleId = userDto.UserRoleId,
                    Active = true
                };
                IdentityResult userAdded;


                if (inviter.UserRoleId == (int)UserRoleEnum.DAM_ADMIN)
                {
                    if (userDto.UserRoleId == (int)UserRoleEnum.COMPANY_ADMIN || userDto.UserRoleId == (int)UserRoleEnum.SUBSCRIBER)
                    {
                        newUser.CompanyId = userDto.CompanyId;
                    }
                }
                else if (inviter.UserRoleId == (int)UserRoleEnum.COMPANY_ADMIN)
                {
                    if (userDto.UserRoleId == (int)UserRoleEnum.COMPANY_ADMIN || userDto.UserRoleId == (int)UserRoleEnum.SUBSCRIBER)
                    {
                        newUser.CompanyId = inviter.CompanyId ?? userDto.CompanyId;
                    }
                }

                userAdded = await _userManager.CreateAsync(newUser);

                if (userAdded.Succeeded)
                {
                    //add folder if subscriber
                    if (userDto.UserRoleId == (int)UserRoleEnum.SUBSCRIBER)
                    {
                        var foldersToAccess = new List<UserFolder>();
                        foreach (var folder in userDto.RootFolderIds)
                        {
                            var f = new UserFolder()
                            {
                                FolderId = folder,
                                UserId = newUser.Id
                            };
                            foldersToAccess.Add(f);
                        }

                        if (foldersToAccess.Count > 0)
                        {
                            _dbcontext.UserFolders.AddRange(foldersToAccess);
                            _dbcontext.SaveChanges();
                        }
                    }
                    else if (userDto.UserRoleId == (int)UserRoleEnum.User && userDto.RootFolderIds != null)
                    {
                        var foldersToAccess = new List<UserFolder>();
                        foreach (var folder in userDto.RootFolderIds)
                        {
                            var f = new UserFolder()
                            {
                                FolderId = folder,
                                UserId = newUser.Id
                            };
                            foldersToAccess.Add(f);
                        }

                        if (foldersToAccess.Count > 0)
                        {
                            _dbcontext.UserFolders.AddRange(foldersToAccess);
                            _dbcontext.SaveChanges();
                        }
                    }

                    // call 
                    var token = await _userManager.GenerateEmailConfirmationTokenAsync(newUser);

                    var confirmationLink = Url.Action("ConfirmEmail", "Users", new { userId = newUser.Id, token = token,  userEmail = newUser.Email}, Request.Scheme);

                    // Send email

                    _emailService.SaveEmailVerificationToEmailQueue(newUser, inviter, confirmationLink);

                    return Ok(new
                    {
                        User = newUser
                        //ConfirmationLink = confirmationLink
                    });
                }
                else
                {
                    var errorMessage = userAdded.Errors.ToList()[0].Description;
                    return BadRequest(new
                    {
                        Message = errorMessage
                    });
                }
            } 
            else
            {
                if (inviter != null && !inviter.UserRole.CanInvite)
                {
                    return Unauthorized();
                }

                return BadRequest(new
                {
                    Message = AppMessageConstants.EmailAlreadyInUse
                });
            }
           
        }

        [ProducesResponseType(typeof(HandlerResult<UserDto>), StatusCodes.Status200OK)]
        [HttpPut]
        [Route("AdminUpdate")]
        public async Task<IActionResult> UpdateUserDetailsByAdmin(UpdateUserDto userDto)
        {
            var userId = _userManager.GetUserId(User);
            var existingUser = _dbcontext.AppUsers.Where(u => u.Email == userDto.Email && !u.Deleted).FirstOrDefault();
            var sendEmailStatusChange = false;

            var currentUser = await _userManager.FindByIdAsync(userId);

            if (currentUser.UserRoleId == (int)UserRoleEnum.SUBSCRIBER)
            {
                return Unauthorized();
            }

            if (existingUser == null)
            {
                return NotFound(new
                {
                    Message = "User does not exist."
                });
            }

            var prevUserRoleId = existingUser.UserRoleId;
            var prevCompanyId = existingUser.CompanyId;
            var prevStatus = existingUser.Active;

            existingUser.UserName = userDto.Username;
            existingUser.Email = userDto.Email;

            #region STATUS 
            existingUser.Active = userDto.Status;
            if (prevStatus != userDto.Status)
            {
                //trigger reactivation/deactivation email
                sendEmailStatusChange = true;
            }

            if (!sendEmailStatusChange)
            {
                existingUser.UserRoleId = userDto.UserRoleId;

                if (currentUser.UserRoleId == (int)UserRoleEnum.COMPANY_ADMIN)
                {
                    existingUser.CompanyId = currentUser.CompanyId;
                } else
                {
                    existingUser.CompanyId = userDto.Company;
                }

                #region USER ROLE UPDATES
                // Check if role and/or company change was done
                if (prevUserRoleId != userDto.UserRoleId && prevCompanyId != userDto.Company)
                {
                    //trigger role and company change email

                }
                else if (prevUserRoleId != userDto.UserRoleId && prevCompanyId == userDto.Company)
                {
                    // trigger role change email
                }
                else if (prevUserRoleId == userDto.UserRoleId && prevCompanyId != userDto.Company)
                {
                    // trigger company change email
                }


                //if ((prevUserRoleId == (int)UserRoleEnum.COMPANY_ADMIN || prevUserRoleId == (int)UserRoleEnum.SUBSCRIBER) && userDto.UserRoleId == (int)UserRoleEnum.DAM_ADMIN)
                //{
                //    existingUser.CompanyId = null;
                //}

                #endregion

                #region USER FOLDER UPDATES
                // check if user role was changed from subscriber to company or admin, delete all folders 
                if ((prevUserRoleId == (int)UserRoleEnum.SUBSCRIBER 
                    && (userDto.UserRoleId == (int)UserRoleEnum.COMPANY_ADMIN 
                        || userDto.UserRoleId == (int)UserRoleEnum.DAM_ADMIN))
                    || prevUserRoleId == (int)UserRoleEnum.User
                    )
                {
                    var prevUserFolders = _dbcontext.UserFolders.Where(up => up.UserId == existingUser.Id).ToList();

                    _dbcontext.UserFolders.RemoveRange(prevUserFolders);
                    _dbcontext.SaveChanges();
                }

                //check user folders modify folders only if subscriber
                if (userDto.UserRoleId == (int)UserRoleEnum.SUBSCRIBER || userDto.UserRoleId == (int)UserRoleEnum.User)
                {
                    // delete old folders
                    var prevUserFolders = _dbcontext.UserFolders.Where(up => up.UserId == existingUser.Id).ToList();

                    _dbcontext.UserFolders.RemoveRange(prevUserFolders);

                    var foldersToAccess = new List<UserFolder>();
                    foreach (var folder in userDto.RootFolderIds)
                    {
                        var f = new UserFolder()
                        {
                            FolderId = folder,
                            UserId = existingUser.Id
                        };
                        foldersToAccess.Add(f);
                    }

                    if (foldersToAccess.Count > 0)
                    {
                        _dbcontext.UserFolders.AddRange(foldersToAccess);

                    }
                    _dbcontext.SaveChanges();
                }
                #endregion
            }
            #endregion

            _dbcontext.AppUsers.Update(existingUser);
            _dbcontext.SaveChanges();

            #region Approvals

            // check if user is part of any approval template when changing role from can approve to subscriber

            if ((prevUserRoleId == (int)UserRoleEnum.COMPANY_ADMIN || prevUserRoleId == (int)UserRoleEnum.DAM_ADMIN) && existingUser.UserRoleId == (int)UserRoleEnum.SUBSCRIBER)
            {
                var approvalTemplateApprovers = _dbcontext.ApprovalTemplateLevelApprovers.Where(x => x.ApproverId == existingUser.Id).ToList();
                var approvalTemplateLevelIds = approvalTemplateApprovers.Select(x => x.ApprovalTemplateLevelId).ToList();
                var approvalTemplateLevels = _dbcontext.ApprovalTemplateLevels.Where(x => approvalTemplateLevelIds.Contains(x.Id)).ToList();

                // delete all rows from template
                if (approvalTemplateApprovers.Count() > 0)
                {
                    _dbcontext.ApprovalTemplateLevelApprovers.RemoveRange(approvalTemplateApprovers);
                    _dbcontext.SaveChanges();
                }

                var defaultDelegateOOOs = _dbcontext.UserOOO.Where(o => o.DefaultDelegateUser == existingUser.Id).ToList();

                var ooosToUpdate = new List<UserOOO>();
                // remove from all default delegate lists
                if (defaultDelegateOOOs.Count() > 0)
                {
                    foreach (var delegateOOO in defaultDelegateOOOs)
                    {
                        var ooo = delegateOOO;
                        ooo.DefaultDelegateUser = null;
                        ooosToUpdate.Add(ooo);
                    }

                    _dbcontext.UserOOO.UpdateRange(ooosToUpdate);
                    _dbcontext.SaveChanges();
                }
            }
               
            #endregion

            //if (sendEmailStatusChange)
            //{
            //    _emailService.SendStatusChangeEmail(userDto);
            //}

            return Ok();
        }


        [ProducesResponseType(typeof(HandlerResult<UserDto>), StatusCodes.Status200OK)]
        [HttpPost]
        [Route("Update")]
        public async Task<IActionResult> UpdateUserDetails(UpdateUserDto userDto)
        {
            var userId = _userManager.GetUserId(User);
            var existingUser = _dbcontext.AppUsers.Where(u => u.Email == userDto.Email && !u.Deleted).FirstOrDefault();

            var currentUser = await _userManager.FindByIdAsync(userId);

            if (existingUser == null)
            {
                return NotFound(new
                {
                    Message = "User does not exist."
                });
            }

            existingUser.UserName = userDto.Username;
            _dbcontext.AppUsers.Update(existingUser);
            _dbcontext.SaveChanges();

            return Ok();
        }

        [ProducesResponseType(typeof(HandlerResult<List<UserDto>>), StatusCodes.Status200OK)]
        [HttpGet]
        [Route("UserFolder/{id}")]
        public async Task<IActionResult> GetUserFolder(string id)
        {
            var result = await _mediator.Send(new GetUserFolderRequest(id));

            return Ok(new
            {
                UserFolders = result.Entity,
            });
        }

        [HttpGet]
        [Route("ConfirmEmail")]
        public async Task<IActionResult> ConfirmEmail(string userId, string token)
        {
            if (userId == null || token == null)
            {
                return BadRequest(new
                {
                    Message = AppMessageConstants.EmailConfirmationFailed + AppMessageConstants.InvalidInputs
                }); ;
            }

            var user = await _userManager.FindByIdAsync(userId);

            if (user == null)
            {
                return NotFound(
                    new
                    {
                        Message = AppMessageConstants.EmailConfirmationFailed + AppMessageConstants.InvalidEmail
                    });
            }

            var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            var result = await _userManager.ConfirmEmailAsync(user, code);

            if (result.Succeeded)
            {
                return Redirect(_baseUrl + "createpassword?email="+user.Email);
            }
            else
            {
                return BadRequest(new
                {
                    Message = AppMessageConstants.EmailConfirmationFailed + result.Errors.First().Description
                });
            }
        }

        [HttpPost]
        [Route("CreatePassword")]
        public async Task<IActionResult> AddPassword(ResetPasswordDto resetPasswordDetails)
        {
            var user = await _userManager.FindByEmailAsync(resetPasswordDetails.Email);

            if (user == null)
            {
                return NotFound(new
                {
                    Message = AppMessageConstants.AddPasswordFailed + AppMessageConstants.InvalidEmail
                });
            }

            var isEmailConfirmed = await _userManager.IsEmailConfirmedAsync(user);

            if (!isEmailConfirmed)
            {
                return BadRequest(new
                {
                    Message = AppMessageConstants.AddPasswordFailed + AppMessageConstants.EmailUnconfirmed
                });
            }

            if (!string.IsNullOrEmpty(user.PasswordHash))
            {
                return BadRequest(new
                {
                    Message = AppMessageConstants.AddPasswordFailed + AppMessageConstants.HasPassword
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
                    Message = AppMessageConstants.AddPasswordFailed + result.Errors.First().Description
                });
            }
        }

        [ProducesResponseType(typeof(HandlerResult<UserDto>), StatusCodes.Status200OK)]
        [Authorize(AuthenticationSchemes = JwtConfigConstants.AuthSchemes)]
        [HttpPut]
        [Route("ShareFoldertoUser")]
        public async Task<IActionResult> ShareFoldertoUser(UserDto requestData)
        {

            var result = await _mediator.Send(new ShareFoldertoUserRequest(requestData));

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

        //[ProducesResponseType(typeof(HandlerResult<UserDto>), StatusCodes.Status200OK)]
        //[HttpPut]
        //[Route("UpdateApprovalStatus")]
        //public async Task<IActionResult> UpdateApprovalStatus(UserDto userDto)
        //{
        //    var result = await _mediator.Send(new UpdateApprovalStatusRequest(userDto));

        //    if (result.Entity == new UserDto())
        //    {
        //        return NotFound();
        //    }
        //    else
        //    {
        //        return Ok(new
        //        {
        //            UserDto = result.Entity,
        //        });
        //    }
        //}

        //[ProducesResponseType(typeof(HandlerResult<UserDto>), StatusCodes.Status200OK)]
        //[HttpGet]
        //[Route("GetUsersBySecurityRole/{roleId}")]

        //public async Task<IActionResult> GetUsersBySecurityRole(int roleId)
        //{
        //    var result = await _mediator.Send(new GetUsersByRoleRequest(roleId));

        //    if (result.Entity == new UserDto())
        //    {
        //        return NotFound();
        //    }
        //    else
        //    {
        //        return Ok(new
        //        {
        //            UserDto = result.Entity,
        //        });
        //    }
        //}

        [ProducesResponseType(typeof(HandlerResult<List<UserOOODto>>), StatusCodes.Status200OK)]
        [Authorize(AuthenticationSchemes = JwtConfigConstants.AuthSchemes)]
        [HttpGet]
        [Route("OOO/{id}")]
        public async Task<IActionResult> GetUserOOO(string id)
        {
            var result = await _mediator.Send(new GetUserOOORequest(id));

            return Ok(new
            {
                UserOOO = result.Entity
            });
        }

        [ProducesResponseType(typeof(HandlerResult<UserOOODto>), StatusCodes.Status200OK)]
        [Authorize(AuthenticationSchemes = JwtConfigConstants.AuthSchemes)]
        [HttpPost]
        [Route("OOO/Add")]
        public async Task<IActionResult> AddUserOOO(UserOOODto userOOO)
        {
            var result = await _mediator.Send(new AddUserOOORequest(userOOO));
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
                UserOOO = result.Entity
            });
        }

        [ProducesResponseType(typeof(HandlerResult<UserOOODto>), StatusCodes.Status200OK)]
        [Authorize(AuthenticationSchemes = JwtConfigConstants.AuthSchemes)]
        [HttpPost]
        [Route("OOO/Edit")]
        public async Task<IActionResult> EditUserOOO(UserOOODto userOOO)
        {
            var result = await _mediator.Send(new EditUserOOORequest(userOOO));
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
                UserOOO = result.Entity
            });
        }
    }
}