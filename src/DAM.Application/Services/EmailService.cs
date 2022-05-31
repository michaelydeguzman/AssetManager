using AutoMapper;
using DAM.Application.Assets.Dtos;
using DAM.Application.Emails.Enums;
using DAM.Application.Projects.Dtos;
using DAM.Application.Services.Interfaces;
using DAM.Application.Users.Constants;
using DAM.Application.Users.Dtos;
using DAM.Domain.Entities;
using DAM.Domain.Entities.Identity;
using DAM.Persistence;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Web;
using System.Xml.Linq;
using static DAM.Application.Services.Interfaces.IEmailService;

namespace DAM.Application.Services
{
    public class EmailService : IEmailService
    {
        private readonly IDbContext _dbcontext;
        private readonly IMapper _mapper;
        private readonly IConfiguration _configuration;
        private readonly IAzureStorageService _azureStorageService;

        public EmailService(IDbContext dbcontext, IMapper mapper, IConfiguration configuration, IAzureStorageService azureStorageService)
        {
            _dbcontext = dbcontext;
            _mapper = mapper;
            _configuration = configuration;
            _azureStorageService = azureStorageService;
        }

        private EmailServiceConfig SetEmailConfig(IConfiguration configuration)
        {
            return new EmailServiceConfig()
            {
                SMTPHost = configuration["SMTPHost"],
                SMTPPort = Convert.ToInt32(configuration["SMTPPort"]),
                SMTPUsername = configuration["SMTPUsername"],
                SMTPPassword = configuration["SMTPPassword"],
                DefaultEmail = configuration["DefaultEmail"],
            };
        }

        public void SendEmail(IConfiguration configuration, string subject, string to, string body)
        {
            var style = _dbcontext.Themes.FirstOrDefault(s => s.IsApplied);
            var logo = _dbcontext.Logos.FirstOrDefault(l => l.IsApplied);

            EmailServiceConfig config = SetEmailConfig(configuration);

            try
            {
                SmtpClient SmtpServer = new SmtpClient(config.SMTPHost, config.SMTPPort);
                SmtpServer.UseDefaultCredentials = false;
                SmtpServer.DeliveryMethod = SmtpDeliveryMethod.Network;
                SmtpServer.Credentials = new System.Net.NetworkCredential(config.SMTPUsername, config.SMTPPassword);

                MailMessage mail = new MailMessage(config.DefaultEmail, to);
                mail.Subject = subject;
                mail.Body = body;
                mail.IsBodyHtml = true;
                SmtpServer.Send(mail);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void AddToEmailQueue(EmailQueue emailQueue)
        {
            _dbcontext.EmailQueues.Add(emailQueue);
            _dbcontext.SaveChanges();
        }

        public string GetEmailHeaderUrl(int emailClassification)
        {
            switch (emailClassification)
            {
                case (int)EmailClassifications.Action:
                    return _azureStorageService.GetEmailHeaderUrl(_configuration, "Action.png");
                case (int)EmailClassifications.Announcement:
                    return _azureStorageService.GetEmailHeaderUrl(_configuration, "Announcement.png");
                case (int)EmailClassifications.Digest:
                    return _azureStorageService.GetEmailHeaderUrl(_configuration, "Digest.png");
                default:
                    return _azureStorageService.GetEmailHeaderUrl(_configuration, "Reminder.png");
            }
        }

        public void SaveEmailVerificationToEmailQueue(ApplicationUser invitee, ApplicationUser inviter, string confirmationLink)
        {
            var emailTemplate = _dbcontext.EmailTemplates.FirstOrDefault(x => x.EmailTemplateKey == EmailTemplateKeys.UserVerificationEmail);

            if (emailTemplate != null)
            {
                var baseUrl = _configuration["BaseUrl"];

                var subject = emailTemplate.Subject;
                var body = emailTemplate.Contents
                    .Replace("%%EmailHeader%%", GetEmailHeaderUrl(emailTemplate.Classification))
                    .Replace("%%To%%", invitee.UserName)
                    .Replace("%%ConfirmEmailUrl%%", confirmationLink)
                    .Replace("%%Message%%", emailTemplate.Message)
                    .Replace("%%ContactUsUrl%%", baseUrl + "ContactUs");

                var newEmailQueue = new EmailQueue()
                {
                    Subject = subject,
                    Contents = body,
                    ToAddress = invitee.Email,
                    FromAddress = _configuration["DefaultEmail"],
                    CreatedById = inviter.Id,
                    Processed = false,
                    Error = false,
                    ErrorMessage = string.Empty,
                    RetryCount = 0,
                    EmailTemplateKey = EmailTemplateKeys.UserVerificationEmail
                };

                AddToEmailQueue(newEmailQueue);

                // Temp until email server is configured
                SendEmail(_configuration, subject, invitee.Email, body);
            }
        }

        public void SaveForgotPasswordToEmailQueue(ApplicationUser user, string token)
        {
            var emailTemplate = _dbcontext.EmailTemplates.FirstOrDefault(x => x.EmailTemplateKey == EmailTemplateKeys.PasswordResetEmail);

            if (emailTemplate != null)
            {
                var baseUrl = _configuration["BaseUrl"];

                var subject = emailTemplate.Subject;
                var body = emailTemplate.Contents
                    .Replace("%%EmailHeader%%", GetEmailHeaderUrl(emailTemplate.Classification))
                    .Replace("%%To%%", user.UserName)
                    .Replace("%%ResetPasswordUrl%%", baseUrl + "resetpassword?email=" + user.Email + "&token=" + HttpUtility.UrlEncode(token))
                    .Replace("%%Message%%", emailTemplate.Message)
                    .Replace("%%ContactUsUrl%%", baseUrl + "ContactUs");

                var newEmailQueue = new EmailQueue()
                {
                    Subject = subject,
                    Contents = body,
                    ToAddress = user.Email,
                    FromAddress = _configuration["DefaultEmail"],
                    CreatedById = user.Id,
                    Processed = false,
                    Error = false,
                    ErrorMessage = string.Empty,
                    RetryCount = 0,
                    EmailTemplateKey = EmailTemplateKeys.PasswordResetEmail
                };

                AddToEmailQueue(newEmailQueue);

                // Temp until email server is configured
                SendEmail(_configuration, subject, user.Email, body);
            }
        }

        public void SaveApprovalEmailToEmailQueue(ApprovalLevel level, List<ApprovalLevelApprover> approvers, string userId, bool isProject, int sourceId, string sourceName)
        {
            var allApprovalLevelApprovers = _dbcontext.ApprovalLevelApprovers.ToList();
            var allApprovalLevels = _dbcontext.ApprovalLevels.ToList();

            var previousApprovalLevelApprovers = (from approvalLevel in allApprovalLevels
                                                  join approvalLevelApprover in allApprovalLevelApprovers
                                                  on approvalLevel.Id equals approvalLevelApprover.ApprovalLevelId
                                                  where approvalLevel.AssetId == level.AssetId && approvalLevel.Id < level.Id
                                                  select approvalLevelApprover).ToList();

            var sendEmailTemplate = _dbcontext.EmailTemplates.FirstOrDefault(a => a.EmailTemplateKey == EmailTemplateKeys.AssetSentForApprovalEmail);
            var resendEmailTemplate = _dbcontext.EmailTemplates.FirstOrDefault(a => a.EmailTemplateKey == EmailTemplateKeys.AssetResentForApprovalEmail);

            var baseUrl = _configuration["BaseUrl"];

            if (sendEmailTemplate != null && resendEmailTemplate != null)
            {
                var users = _dbcontext.AppUsers.Where(u => !u.Deleted).ToList();
                var asset = _dbcontext.Assets.FirstOrDefault(a => a.Id == level.AssetId);
                var assetVersion = _dbcontext.AssetVersions.FirstOrDefault(av => av.Id == level.AssetVersionId);
                var submitter = users.First(u => u.Id == userId);

                var assetTable = new XElement("table");
                assetTable.Add(new XAttribute("cellpadding", 12));
                assetTable.Add(new XAttribute("border", 1));
                assetTable.Add(new XAttribute("style", "border-collapse:collapse"));

                var headers = new XElement("tr");
                headers.Add(new XElement("th", new XElement("span", "Asset Name")));
                headers.Add(new XElement("th", new XElement("span", isProject ? "Project" : "Folder")));
                headers.Add(new XElement("th", new XElement("span", "Approval Requested By")));

                var assetDetails = new XElement("tr");
                assetDetails.Add(new XElement("td",
                                    new XElement("span",
                                        new XElement("a",
                                            new XAttribute("href",
                                                baseUrl + $"approvals/{asset.Id}"),
                                            asset.Title
                                        )
                                    )
                                  )
                                );
                var sourceUrl = isProject ? "projects" : "assets/folder";
                assetDetails.Add(new XElement("td",
                                    new XElement("span",
                                        new XElement("a",
                                            new XAttribute("href",
                                                baseUrl + $"{sourceUrl}/{sourceId}"),
                                            sourceName
                                        )
                                    )
                                  )
                                );

                assetDetails.Add(new XElement("td", new XElement("span", submitter.UserName)));

                if (level.DueDate != null)
                {
                    headers.Add(new XElement("th", new XElement("span", "Approval Due Date")));
                    assetDetails.Add(new XElement("td", new XElement("span", level.DueDate.Value)));
                }

                assetTable.Add(headers);
                assetTable.Add(assetDetails);

                foreach (var approver in approvers.Where(a => a.ApprovalLevelId == level.Id))
                {
                    var user = users.First(u => u.Id == approver.ApproverId);
                    var resend = false;

                    if (previousApprovalLevelApprovers.Count > 0)
                    {
                        if (previousApprovalLevelApprovers.Where(x => x.ReviewDate != null).Select(x => x.ApproverId).Contains(approver.ApproverId))
                        {
                            resend = true;
                        }
                    }

                    var uri = baseUrl + "approvals/" + level.AssetId;

                    var body = sendEmailTemplate.Contents
                        .Replace("%%EmailHeader%%", GetEmailHeaderUrl(resend ? resendEmailTemplate.Classification : sendEmailTemplate.Classification))
                        .Replace("%%To%%", user.UserName)
                        .Replace("%%Message%%", resend ? resendEmailTemplate.Message : sendEmailTemplate.Message)
                        .Replace("%%ApprovalsUrl%%", baseUrl + "approvals")
                        .Replace("%%AssetTable%%", assetTable.ToString());

                    var newEmailQueue = new EmailQueue()
                    {
                        Subject = resend ? resendEmailTemplate.Subject : sendEmailTemplate.Subject,
                        Contents = body,
                        ToAddress = user.Email,
                        FromAddress = _configuration["DefaultEmail"],
                        CreatedById = submitter.Id,
                        Processed = false,
                        Error = false,
                        ErrorMessage = string.Empty,
                        RetryCount = 0,
                        EmailTemplateKey = resend ? EmailTemplateKeys.AssetResentForApprovalEmail : EmailTemplateKeys.AssetSentForApprovalEmail,
                        AssetId = asset.Id,
                        AssetVersionId = assetVersion.Id
                    };

                    if (isProject)
                    {
                        newEmailQueue.ProjectId = sourceId;
                    }
                    else
                    {
                        newEmailQueue.FolderId = sourceId;
                    }

                    AddToEmailQueue(newEmailQueue);
                    // Temp until email server is configured
                    SendEmail(_configuration, resend ? resendEmailTemplate.Subject : sendEmailTemplate.Subject, user.Email, body);
                }
            }
        }

        public void SaveApprovedEmailToEmailQueue(Asset asset, AssetVersions assetVersion, string approverId, string userId, string comments, bool isProject, int sourceId, string sourceName)
        {
            var emailTemplate = _dbcontext.EmailTemplates.FirstOrDefault(a => a.EmailTemplateKey == EmailTemplateKeys.AssetApproved);

            if (emailTemplate != null)
            {
                var users = _dbcontext.AppUsers.Where(u => !u.Deleted).ToList();

                var baseUrl = _configuration["BaseUrl"];
                var approver = users.First(u => u.Id == approverId);
                var submitter = users.First(u => u.Id == userId);
                var uri = baseUrl + "assets/" + asset.Id;

                var assetTable = new XElement("table");
                assetTable.Add(new XAttribute("cellpadding", 12));
                assetTable.Add(new XAttribute("border", 1));
                assetTable.Add(new XAttribute("style", "border-collapse:collapse"));

                var headers = new XElement("tr");
                headers.Add(new XElement("th", new XElement("span", "Asset Name")));
                headers.Add(new XElement("th", new XElement("span", isProject ? "Project" : "Folder")));
                headers.Add(new XElement("th", new XElement("span", "Approved By")));

                var assetDetails = new XElement("tr");
                assetDetails.Add(new XElement("td",
                                    new XElement("span",
                                        new XElement("a",
                                            new XAttribute("href",
                                                baseUrl + $"assets/{asset.Id}"),
                                            asset.Title
                                        )
                                    )
                                  )
                                );
                var sourceUrl = isProject ? "projects" : "assets/folder";
                assetDetails.Add(new XElement("td",
                                    new XElement("span",
                                        new XElement("a",
                                            new XAttribute("href",
                                                baseUrl + $"{sourceUrl}/{sourceId}"),
                                            sourceName
                                        )
                                    )
                                  )
                                );

                assetDetails.Add(new XElement("td", new XElement("span", approver.UserName)));


                assetTable.Add(headers);
                assetTable.Add(assetDetails);

                var body = emailTemplate.Contents
                                       .Replace("%%EmailHeader%%", GetEmailHeaderUrl(emailTemplate.Classification))
                                       .Replace("%%To%%", submitter.UserName)
                                       .Replace("%%Message%%", emailTemplate.Message)
                                       .Replace("%%FeedbackMessage%%", comments)
                                       .Replace("%%AssetTable%%", assetTable.ToString());

                var newEmailQueue = new EmailQueue()
                {
                    Subject = emailTemplate.Subject,
                    Contents = body,
                    ToAddress = submitter.Email,
                    FromAddress = _configuration["DefaultEmail"],
                    CreatedById = approver.Id,
                    Processed = false,
                    Error = false,
                    ErrorMessage = string.Empty,
                    RetryCount = 0,
                    EmailTemplateKey = EmailTemplateKeys.AssetApproved,
                    AssetId = asset.Id,
                    AssetVersionId = assetVersion.Id
                };

                if (isProject)
                {
                    newEmailQueue.ProjectId = sourceId;
                }
                else
                {
                    newEmailQueue.FolderId = sourceId;
                }

                AddToEmailQueue(newEmailQueue);

                // Temp until email server is configured
                SendEmail(_configuration, emailTemplate.Subject, submitter.Email, body);
            }
        }

        public void SaveRejectedEmailToEmailQueue(Asset asset, AssetVersions assetVersion, string approverId, string userId, string comments, bool isProject, int sourceId, string sourceName)
        {
            var emailTemplate = _dbcontext.EmailTemplates.FirstOrDefault(a => a.EmailTemplateKey == EmailTemplateKeys.AssetRejected);

            if (emailTemplate != null)
            {
                var users = _dbcontext.AppUsers.Where(u => !u.Deleted).ToList();

                var baseUrl = _configuration["BaseUrl"];
                var approver = users.First(u => u.Id == approverId);
                var submitter = users.First(u => u.Id == userId);
                var uri = baseUrl + "assets/" + asset.Id;

                var assetTable = new XElement("table");
                assetTable.Add(new XAttribute("cellpadding", 12));
                assetTable.Add(new XAttribute("border", 1));
                assetTable.Add(new XAttribute("style", "border-collapse:collapse"));

                var headers = new XElement("tr");
                headers.Add(new XElement("th", new XElement("span", "Asset Name")));
                headers.Add(new XElement("th", new XElement("span", isProject ? "Project" : "Folder")));
                headers.Add(new XElement("th", new XElement("span", "Rejected By")));

                var assetDetails = new XElement("tr");
                assetDetails.Add(new XElement("td",
                                    new XElement("span",
                                        new XElement("a",
                                            new XAttribute("href",
                                                baseUrl + $"assets/{asset.Id}"),
                                            asset.Title
                                        )
                                    )
                                  )
                                );
                var sourceUrl = isProject ? "projects" : "assets/folder";
                assetDetails.Add(new XElement("td",
                                    new XElement("span",
                                        new XElement("a",
                                            new XAttribute("href",
                                                baseUrl + $"{sourceUrl}/{sourceId}"),
                                            sourceName
                                        )
                                    )
                                  )
                                );

                assetDetails.Add(new XElement("td", new XElement("span", approver.UserName)));


                assetTable.Add(headers);
                assetTable.Add(assetDetails);

                var body = emailTemplate.Contents
                                       .Replace("%%EmailHeader%%", GetEmailHeaderUrl(emailTemplate.Classification))
                                       .Replace("%%To%%", submitter.UserName)
                                       .Replace("%%Message%%", emailTemplate.Message)
                                       .Replace("%%FeedbackMessage%%", comments)
                                       .Replace("%%AssetTable%%", assetTable.ToString());

                var newEmailQueue = new EmailQueue()
                {
                    Subject = emailTemplate.Subject,
                    Contents = body,
                    ToAddress = submitter.Email,
                    FromAddress = _configuration["DefaultEmail"],
                    CreatedById = approver.Id,
                    Processed = false,
                    Error = false,
                    ErrorMessage = string.Empty,
                    RetryCount = 0,
                    EmailTemplateKey = EmailTemplateKeys.AssetRejected,
                    AssetId = asset.Id,
                    AssetVersionId = assetVersion.Id
                };

                if (isProject)
                {
                    newEmailQueue.ProjectId = sourceId;
                }
                else
                {
                    newEmailQueue.FolderId = sourceId;
                }

                AddToEmailQueue(newEmailQueue);

                // Temp until email server is configured
                SendEmail(_configuration, emailTemplate.Subject, submitter.Email, body);
            }
        }

        public void SaveDelegateApprovalEmailToEmailQueue(ApprovalLevel level, string newApproverId, string oldApproverId)
        {
            var emailTemplate = _dbcontext.EmailTemplates.FirstOrDefault(a => a.EmailTemplateKey == EmailTemplateKeys.AssetDelegatedForApprovalEmail);

            if (emailTemplate != null)
            {
                var asset = _dbcontext.Assets.FirstOrDefault(a => a.Id == level.AssetId);
                var isProject = asset.ProjectId != null;
                var sourceId = 0;
                var sourceName = string.Empty;

                if (isProject)
                {
                    var project = _dbcontext.Projects.FirstOrDefault(p => p.Id == asset.ProjectId);
                    sourceId = project.Id;
                    sourceName = project.ProjectName;
                }
                else
                {
                    var folder = _dbcontext.Folders.FirstOrDefault(p => p.Id == asset.FolderId);
                    sourceId = folder.Id;
                    sourceName = folder.FolderName;
                }

                var assetVersion = _dbcontext.AssetVersions.FirstOrDefault(av => av.Id == level.AssetVersionId);
                var users = _dbcontext.AppUsers.Where(u => !u.Deleted).ToList();

                var baseUrl = _configuration["BaseUrl"];
                var newApprover = users.First(u => u.Id == newApproverId);
                var oldApprover = users.First(u => u.Id == oldApproverId);
                var uri = baseUrl + "approvals/" + asset.Id;

                var assetTable = new XElement("table");
                assetTable.Add(new XAttribute("cellpadding", 12));
                assetTable.Add(new XAttribute("border", 1));
                assetTable.Add(new XAttribute("style", "border-collapse:collapse"));

                var headers = new XElement("tr");
                headers.Add(new XElement("th", new XElement("span", "Asset Name")));
                headers.Add(new XElement("th", new XElement("span", isProject ? "Project" : "Folder")));
                headers.Add(new XElement("th", new XElement("span", "Delegated From")));

                var assetDetails = new XElement("tr");
                assetDetails.Add(new XElement("td",
                                    new XElement("span",
                                        new XElement("a",
                                            new XAttribute("href",
                                                baseUrl + $"approvals/{asset.Id}"),
                                            asset.Title
                                        )
                                    )
                                  )
                                );
                var sourceUrl = isProject ? "projects" : "assets/folder";
                assetDetails.Add(new XElement("td",
                                    new XElement("span",
                                        new XElement("a",
                                            new XAttribute("href",
                                                baseUrl + $"{sourceUrl}/{sourceId}"),
                                            sourceName
                                        )
                                    )
                                  )
                                );

                assetDetails.Add(new XElement("td", new XElement("span", oldApprover.UserName)));

                if (level.DueDate != null)
                {
                    headers.Add(new XElement("th", new XElement("span", "Approval Due Date")));
                    assetDetails.Add(new XElement("td", new XElement("span", level.DueDate.Value)));
                }

                assetTable.Add(headers);
                assetTable.Add(assetDetails);

                var body = emailTemplate.Contents
                    .Replace("%%EmailHeader%%", GetEmailHeaderUrl(emailTemplate.Classification))
                    .Replace("%%To%%", newApprover.UserName)
                    .Replace("%%Message%%", emailTemplate.Message)
                    .Replace("%%ApprovalsUrl%%", baseUrl + "approvals")
                    .Replace("%%AssetTable%%", assetTable.ToString());

                var newEmailQueue = new EmailQueue()
                {
                    Subject = emailTemplate.Subject,
                    Contents = body,
                    ToAddress = newApprover.Email,
                    FromAddress = _configuration["DefaultEmail"],
                    CreatedById = oldApprover.Id,
                    Processed = false,
                    Error = false,
                    ErrorMessage = string.Empty,
                    RetryCount = 0,
                    EmailTemplateKey = EmailTemplateKeys.AssetDelegatedForApprovalEmail,
                    AssetId = asset.Id,
                    AssetVersionId = assetVersion.Id
                };

                if (isProject)
                {
                    newEmailQueue.ProjectId = sourceId;
                }
                else
                {
                    newEmailQueue.FolderId = sourceId;
                }

                AddToEmailQueue(newEmailQueue);


                // Temp until email server is configured
                SendEmail(_configuration, emailTemplate.Subject, newApprover.Email, body);
            }
        }

        public void SaveAddedOwnersToEmailQueue(List<string> newOwnerIds, int projectId, string projectName, string userId)
        {
            var emailTemplate = _dbcontext.EmailTemplates.FirstOrDefault(a => a.EmailTemplateKey == EmailTemplateKeys.ProjectOwnerAdded);
            var allUsers = _dbcontext.AppUsers.Where(x => !x.Deleted && x.Active).ToList();
            var requestedBy = allUsers.FirstOrDefault(x => x.Id == userId);

            if (emailTemplate != null)
            {
                var baseUrl = _configuration["BaseUrl"];
                var projectUrl = baseUrl + "projects/" + projectId;

                var projectTable = new XElement("table");
                projectTable.Add(new XAttribute("cellpadding", 12));
                projectTable.Add(new XAttribute("border", 1));
                projectTable.Add(new XAttribute("style", "border-collapse:collapse"));

                var headers = new XElement("tr");
                headers.Add(new XElement("th", new XElement("span", "Project Name")));
                headers.Add(new XElement("th", new XElement("span", "Added By")));

                var projectDetails = new XElement("tr");
                projectDetails.Add(new XElement("td",
                                    new XElement("span",
                                        new XElement("a",
                                            new XAttribute("href",
                                                projectUrl),
                                            projectName
                                        )
                                    )
                                  )
                                );

                projectDetails.Add(new XElement("td", new XElement("span", requestedBy.UserName)));

                projectTable.Add(headers);
                projectTable.Add(projectDetails);

                var body = emailTemplate.Contents
                            .Replace("%%EmailHeader%%", GetEmailHeaderUrl(emailTemplate.Classification))
                            .Replace("%%Message%%", emailTemplate.Message)
                            .Replace("%%ProjectTable%%", projectTable.ToString());

                foreach (var ownerId in newOwnerIds)
                {
                    if (ownerId != requestedBy.Id)
                    {
                        var newOwner = allUsers.FirstOrDefault(x => x.Id == ownerId);
                        if (newOwner != null)
                        {
                            var finalBody = body.Replace("%%To%%", newOwner.UserName);
                            var newEmailQueue = new EmailQueue()
                            {
                                Subject = emailTemplate.Subject,
                                Contents = finalBody,
                                ToAddress = newOwner.Email,
                                FromAddress = _configuration["DefaultEmail"],
                                CreatedById = requestedBy.Id,
                                Processed = false,
                                Error = false,
                                ErrorMessage = string.Empty,
                                RetryCount = 0,
                                EmailTemplateKey = EmailTemplateKeys.ProjectOwnerAdded,
                                ProjectId = projectId
                            };

                            AddToEmailQueue(newEmailQueue);

                            // Temp until email server is configured
                            SendEmail(_configuration, emailTemplate.Subject, newOwner.Email, body);
                        }
                    }
                };
            }
        }

        public void SaveAddedCollaboratorsToEmailQueue(List<string> newCollaboratorIds, int projectId, string projectName, string userId)
        {
            var emailTemplate = _dbcontext.EmailTemplates.FirstOrDefault(a => a.EmailTemplateKey == EmailTemplateKeys.ProjectCollaboratorAdded);
            var allUsers = _dbcontext.AppUsers.Where(x => !x.Deleted && x.Active).ToList();
            var requestedBy = allUsers.FirstOrDefault(x => x.Id == userId);

            if (emailTemplate != null)
            {
                var baseUrl = _configuration["BaseUrl"];
                var projectUrl = baseUrl + "projects/" + projectId;

                var projectTable = new XElement("table");
                projectTable.Add(new XAttribute("cellpadding", 12));
                projectTable.Add(new XAttribute("border", 1));
                projectTable.Add(new XAttribute("style", "border-collapse:collapse"));

                var headers = new XElement("tr");
                headers.Add(new XElement("th", new XElement("span", "Project Name")));
                headers.Add(new XElement("th", new XElement("span", "Added By")));

                var projectDetails = new XElement("tr");
                projectDetails.Add(new XElement("td",
                                    new XElement("span",
                                        projectName
                                    )
                                  )
                                );

                projectDetails.Add(new XElement("td", new XElement("span", requestedBy.UserName)));

                projectTable.Add(headers);
                projectTable.Add(projectDetails);

                var body = emailTemplate.Contents
                            .Replace("%%EmailHeader%%", GetEmailHeaderUrl(emailTemplate.Classification))
                            .Replace("%%Message%%", emailTemplate.Message)
                            .Replace("%%ProjectTable%%", projectTable.ToString());

                foreach (var collabId in newCollaboratorIds)
                {
                    if (collabId != requestedBy.Id)
                    {
                        var newCollaborator = allUsers.FirstOrDefault(x => x.Id == collabId);
                        if (newCollaborator != null)
                        {
                            var finalBody = body.Replace("%%To%%", newCollaborator.UserName);
                            var newEmailQueue = new EmailQueue()
                            {
                                Subject = emailTemplate.Subject,
                                Contents = finalBody,
                                ToAddress = newCollaborator.Email,
                                FromAddress = _configuration["DefaultEmail"],
                                CreatedById = requestedBy.Id,
                                Processed = false,
                                Error = false,
                                ErrorMessage = string.Empty,
                                RetryCount = 0,
                                EmailTemplateKey = EmailTemplateKeys.ProjectCollaboratorAdded,
                                ProjectId = projectId
                            };

                            AddToEmailQueue(newEmailQueue);

                            // Temp until email server is configured
                            SendEmail(_configuration, emailTemplate.Subject, newCollaborator.Email, finalBody);
                        }
                    }
                };
            }
        }

        public void SaveProjectDeletedOwnerToEmailQueue(List<string> projectOwnerIds, int projectId, string projectName, string userId)
        {
            var emailTemplate = _dbcontext.EmailTemplates.FirstOrDefault(a => a.EmailTemplateKey == EmailTemplateKeys.ProjectDeletedOwner);
            var allUsers = _dbcontext.AppUsers.Where(x => !x.Deleted && x.Active).ToList();
            var requestedBy = allUsers.FirstOrDefault(x => x.Id == userId);

            if (emailTemplate != null)
            {
                var baseUrl = _configuration["BaseUrl"];
                var projectUrl = baseUrl + "projects/" + projectId;

                var projectTable = new XElement("table");
                projectTable.Add(new XAttribute("cellpadding", 12));
                projectTable.Add(new XAttribute("border", 1));
                projectTable.Add(new XAttribute("style", "border-collapse:collapse"));

                var headers = new XElement("tr");
                headers.Add(new XElement("th", new XElement("span", "Project Name")));
                headers.Add(new XElement("th", new XElement("span", "Deleted By")));

                var projectDetails = new XElement("tr");
                projectDetails.Add(new XElement("td",
                                    new XElement("span",
                                        projectName
                                    )
                                  )
                                );

                projectDetails.Add(new XElement("td", new XElement("span", requestedBy.UserName)));

                projectTable.Add(headers);
                projectTable.Add(projectDetails);

                var body = emailTemplate.Contents
                            .Replace("%%EmailHeader%%", GetEmailHeaderUrl(emailTemplate.Classification))
                            .Replace("%%Message%%", emailTemplate.Message)
                            .Replace("%%ProjectTable%%", projectTable.ToString());

                foreach (var ownerId in projectOwnerIds)
                {
                    if (ownerId != requestedBy.Id)
                    {
                        var newOwner = allUsers.FirstOrDefault(x => x.Id == ownerId);
                        if (newOwner != null)
                        {
                            var finalBody = body.Replace("%%To%%", newOwner.UserName);
                            var newEmailQueue = new EmailQueue()
                            {
                                Subject = emailTemplate.Subject,
                                Contents = finalBody,
                                ToAddress = newOwner.Email,
                                FromAddress = _configuration["DefaultEmail"],
                                CreatedById = requestedBy.Id,
                                Processed = false,
                                Error = false,
                                ErrorMessage = string.Empty,
                                RetryCount = 0,
                                EmailTemplateKey = EmailTemplateKeys.ProjectDeletedOwner,
                                ProjectId = projectId
                            };

                            AddToEmailQueue(newEmailQueue);

                            // Temp until email server is configured
                            SendEmail(_configuration, emailTemplate.Subject, newOwner.Email, finalBody);
                        }
                    }
                };
            }
        }

        public void SaveProjectDeletedCollaboratorToEmailQueue(List<string> projectCollaboratorIds, int projectId, string projectName, string userId)
        {
            var emailTemplate = _dbcontext.EmailTemplates.FirstOrDefault(a => a.EmailTemplateKey == EmailTemplateKeys.ProjectDeletedCollaborator);
            var allUsers = _dbcontext.AppUsers.Where(x => !x.Deleted && x.Active).ToList();
            var requestedBy = allUsers.FirstOrDefault(x => x.Id == userId);

            if (emailTemplate != null)
            {
                var baseUrl = _configuration["BaseUrl"];
                var projectUrl = baseUrl + "projects/" + projectId;

                var projectTable = new XElement("table");
                projectTable.Add(new XAttribute("cellpadding", 12));
                projectTable.Add(new XAttribute("border", 1));
                projectTable.Add(new XAttribute("style", "border-collapse:collapse"));

                var headers = new XElement("tr");
                headers.Add(new XElement("th", new XElement("span", "Project Name")));
                headers.Add(new XElement("th", new XElement("span", "Deleted By")));

                var projectDetails = new XElement("tr");
                projectDetails.Add(new XElement("td",
                                    new XElement("span",
                                        projectName
                                    )
                                  )
                                );

                projectDetails.Add(new XElement("td", new XElement("span", requestedBy.UserName)));

                projectTable.Add(headers);
                projectTable.Add(projectDetails);

                var body = emailTemplate.Contents
                            .Replace("%%EmailHeader%%", GetEmailHeaderUrl(emailTemplate.Classification))
                            .Replace("%%Message%%", emailTemplate.Message)
                            .Replace("%%ProjectTable%%", projectTable.ToString());

                foreach (var collabId in projectCollaboratorIds)
                {
                    if (collabId != requestedBy.Id)
                    {
                        var newCollaborator = allUsers.FirstOrDefault(x => x.Id == collabId);
                        if (newCollaborator != null)
                        {
                            var finalBody = body.Replace("%%To%%", newCollaborator.UserName);
                            var newEmailQueue = new EmailQueue()
                            {
                                Subject = emailTemplate.Subject,
                                Contents = finalBody,
                                ToAddress = newCollaborator.Email,
                                FromAddress = _configuration["DefaultEmail"],
                                CreatedById = requestedBy.Id,
                                Processed = false,
                                Error = false,
                                ErrorMessage = string.Empty,
                                RetryCount = 0,
                                EmailTemplateKey = EmailTemplateKeys.ProjectDeletedCollaborator,
                                ProjectId = projectId
                            };

                            AddToEmailQueue(newEmailQueue);

                            // Temp until email server is configured
                            SendEmail(_configuration, emailTemplate.Subject, newCollaborator.Email, finalBody);
                        }
                    }
                };
            }
        }

        public void SaveProjectArchivedOwnerToEmailQueue(List<string> projectOwnerIds, int projectId, string projectName, string userId)
        {
            var emailTemplate = _dbcontext.EmailTemplates.FirstOrDefault(a => a.EmailTemplateKey == EmailTemplateKeys.ProjectArchivedOwner);
            var allUsers = _dbcontext.AppUsers.Where(x => !x.Deleted && x.Active).ToList();
            var requestedBy = allUsers.FirstOrDefault(x => x.Id == userId);

            if (emailTemplate != null)
            {
                var baseUrl = _configuration["BaseUrl"];
                var projectUrl = baseUrl + "projects/" + projectId;

                var projectTable = new XElement("table");
                projectTable.Add(new XAttribute("cellpadding", 12));
                projectTable.Add(new XAttribute("border", 1));
                projectTable.Add(new XAttribute("style", "border-collapse:collapse"));

                var headers = new XElement("tr");
                headers.Add(new XElement("th", new XElement("span", "Project Name")));
                headers.Add(new XElement("th", new XElement("span", "Archived By")));

                var projectDetails = new XElement("tr");
                projectDetails.Add(new XElement("td",
                                    new XElement("span",
                                       projectName
                                    )
                                  )
                                );

                projectDetails.Add(new XElement("td", new XElement("span", requestedBy.UserName)));

                projectTable.Add(headers);
                projectTable.Add(projectDetails);

                var body = emailTemplate.Contents
                            .Replace("%%EmailHeader%%", GetEmailHeaderUrl(emailTemplate.Classification))
                            .Replace("%%Message%%", emailTemplate.Message)
                            .Replace("%%ProjectTable%%", projectTable.ToString());

                foreach (var ownerId in projectOwnerIds)
                {
                    if (ownerId != requestedBy.Id)
                    {
                        var newOwner = allUsers.FirstOrDefault(x => x.Id == ownerId);
                        if (newOwner != null)
                        {
                            var finalBody = body.Replace("%%To%%", newOwner.UserName);
                            var newEmailQueue = new EmailQueue()
                            {
                                Subject = emailTemplate.Subject,
                                Contents = finalBody,
                                ToAddress = newOwner.Email,
                                FromAddress = _configuration["DefaultEmail"],
                                CreatedById = requestedBy.Id,
                                Processed = false,
                                Error = false,
                                ErrorMessage = string.Empty,
                                RetryCount = 0,
                                EmailTemplateKey = EmailTemplateKeys.ProjectArchivedOwner,
                                ProjectId = projectId
                            };

                            AddToEmailQueue(newEmailQueue);

                            // Temp until email server is configured
                            SendEmail(_configuration, emailTemplate.Subject, newOwner.Email, finalBody);
                        }
                    }
                };
            }
        }

        public void SaveProjectArchivedCollaboratorToEmailQueue(List<string> projectCollaboratorIds, int projectId, string projectName, string userId)
        {
            var emailTemplate = _dbcontext.EmailTemplates.FirstOrDefault(a => a.EmailTemplateKey == EmailTemplateKeys.ProjectArchivedCollaborator);
            var allUsers = _dbcontext.AppUsers.Where(x => !x.Deleted && x.Active).ToList();
            var requestedBy = allUsers.FirstOrDefault(x => x.Id == userId);

            if (emailTemplate != null)
            {
                var baseUrl = _configuration["BaseUrl"];
                var projectUrl = baseUrl + "projects/" + projectId;

                var projectTable = new XElement("table");
                projectTable.Add(new XAttribute("cellpadding", 12));
                projectTable.Add(new XAttribute("border", 1));
                projectTable.Add(new XAttribute("style", "border-collapse:collapse"));

                var headers = new XElement("tr");
                headers.Add(new XElement("th", new XElement("span", "Project Name")));
                headers.Add(new XElement("th", new XElement("span", "Archived By")));

                var projectDetails = new XElement("tr");
                projectDetails.Add(new XElement("td",
                                    new XElement("span",
                                      projectName
                                    )
                                  )
                                );

                projectDetails.Add(new XElement("td", new XElement("span", requestedBy.UserName)));

                projectTable.Add(headers);
                projectTable.Add(projectDetails);

                var body = emailTemplate.Contents
                            .Replace("%%EmailHeader%%", GetEmailHeaderUrl(emailTemplate.Classification))
                            .Replace("%%Message%%", emailTemplate.Message)
                            .Replace("%%ProjectTable%%", projectTable.ToString());

                foreach (var collabId in projectCollaboratorIds)
                {
                    if (collabId != requestedBy.Id)
                    {
                        var newCollaborator = allUsers.FirstOrDefault(x => x.Id == collabId);
                        if (newCollaborator != null)
                        {
                            var finalBody = body.Replace("%%To%%", newCollaborator.UserName);
                            var newEmailQueue = new EmailQueue()
                            {
                                Subject = emailTemplate.Subject,
                                Contents = finalBody,
                                ToAddress = newCollaborator.Email,
                                FromAddress = _configuration["DefaultEmail"],
                                CreatedById = requestedBy.Id,
                                Processed = false,
                                Error = false,
                                ErrorMessage = string.Empty,
                                RetryCount = 0,
                                EmailTemplateKey = EmailTemplateKeys.ProjectArchivedCollaborator,
                                ProjectId = projectId
                            };

                            AddToEmailQueue(newEmailQueue);

                            // Temp until email server is configured
                            SendEmail(_configuration, emailTemplate.Subject, newCollaborator.Email, finalBody);
                        }
                    }
                };
            }
        }

        public void SaveProjectUpdatedOwnerToEmailQueue(List<string> projectOwnerIds, List<ProjectUpdateDetailsDto> details, int projectId, string projectName, string userId)
        {
            var emailTemplate = _dbcontext.EmailTemplates.FirstOrDefault(a => a.EmailTemplateKey == EmailTemplateKeys.ProjectUpdatedOwner);
            var allUsers = _dbcontext.AppUsers.Where(x => !x.Deleted && x.Active).ToList();
            var requestedBy = allUsers.FirstOrDefault(x => x.Id == userId);

            if (emailTemplate != null)
            {
                var baseUrl = _configuration["BaseUrl"];
                var projectUrl = baseUrl + "projects/" + projectId;

                var projectDetailsTable = new XElement("table");
                projectDetailsTable.Add(new XAttribute("cellpadding", 12));
                projectDetailsTable.Add(new XAttribute("border", 1));
                projectDetailsTable.Add(new XAttribute("style", "border-collapse:collapse"));

                var headers = new XElement("tr");
                headers.Add(new XElement("th", new XElement("span", "Modified Field")));
                headers.Add(new XElement("th", new XElement("span", "Old Value")));
                headers.Add(new XElement("th", new XElement("span", "New Value")));
                headers.Add(new XElement("th", new XElement("span", "Modified By")));

                projectDetailsTable.Add(headers);

                foreach (var detail in details)
                {
                    var projectDetails = new XElement("tr");
                    projectDetails.Add(new XElement("td", new XElement("span", detail.FieldName)));
                    projectDetails.Add(new XElement("td", new XElement("span", detail.OldValue)));
                    projectDetails.Add(new XElement("td", new XElement("span", detail.NewValue)));
                    projectDetails.Add(new XElement("td", new XElement("span", requestedBy.UserName)));
                    projectDetailsTable.Add(projectDetails);
                }

                var body = emailTemplate.Contents
                            .Replace("%%EmailHeader%%", GetEmailHeaderUrl(emailTemplate.Classification))
                            .Replace("%%ProjectName%%", projectName)
                            .Replace("%%ProjectUrl%%", projectUrl)
                            .Replace("%%Message%%", emailTemplate.Message)
                            .Replace("%%ProjectDetailsTable%%", projectDetailsTable.ToString());

                foreach (var ownerId in projectOwnerIds)
                {
                    if (ownerId != requestedBy.Id)
                    {
                        var owner = allUsers.FirstOrDefault(x => x.Id == ownerId);
                        if (owner != null)
                        {
                            var finalBody = body.Replace("%%To%%", owner.UserName);
                            var newEmailQueue = new EmailQueue()
                            {
                                Subject = emailTemplate.Subject,
                                Contents = finalBody,
                                ToAddress = owner.Email,
                                FromAddress = _configuration["DefaultEmail"],
                                CreatedById = requestedBy.Id,
                                Processed = false,
                                Error = false,
                                ErrorMessage = string.Empty,
                                RetryCount = 0,
                                EmailTemplateKey = EmailTemplateKeys.ProjectUpdatedOwner,
                                ProjectId = projectId
                            };

                            AddToEmailQueue(newEmailQueue);

                            // Temp until email server is configured
                            SendEmail(_configuration, emailTemplate.Subject, owner.Email, finalBody);
                        }
                    }
                };
            }
        }

        public void SaveProjectUpdatedCollaboratorToEmailQueue(List<string> projectCollaboratorIds, List<ProjectUpdateDetailsDto> details, int projectId, string projectName, string userId)
        {
            var emailTemplate = _dbcontext.EmailTemplates.FirstOrDefault(a => a.EmailTemplateKey == EmailTemplateKeys.ProjectUpdatedCollaborator);
            var allUsers = _dbcontext.AppUsers.Where(x => !x.Deleted && x.Active).ToList();
            var requestedBy = allUsers.FirstOrDefault(x => x.Id == userId);

            if (emailTemplate != null)
            {
                var baseUrl = _configuration["BaseUrl"];
                var projectUrl = baseUrl + "projects/" + projectId;

                var projectDetailsTable = new XElement("table");
                projectDetailsTable.Add(new XAttribute("cellpadding", 12));
                projectDetailsTable.Add(new XAttribute("border", 1));
                projectDetailsTable.Add(new XAttribute("style", "border-collapse:collapse"));

                var headers = new XElement("tr");
                headers.Add(new XElement("th", new XElement("span", "Modified Field")));
                headers.Add(new XElement("th", new XElement("span", "Old Value")));
                headers.Add(new XElement("th", new XElement("span", "New Value")));
                headers.Add(new XElement("th", new XElement("span", "Modified By")));

                projectDetailsTable.Add(headers);

                foreach (var detail in details)
                {
                    var projectDetails = new XElement("tr");
                    projectDetails.Add(new XElement("td", new XElement("span", detail.FieldName)));
                    projectDetails.Add(new XElement("td", new XElement("span", detail.OldValue)));
                    projectDetails.Add(new XElement("td", new XElement("span", detail.NewValue)));
                    projectDetails.Add(new XElement("td", new XElement("span", requestedBy.UserName)));
                    projectDetailsTable.Add(projectDetails);
                }

                var body = emailTemplate.Contents
                            .Replace("%%EmailHeader%%", GetEmailHeaderUrl(emailTemplate.Classification))
                            .Replace("%%ProjectName%%", projectName)
                            .Replace("%%ProjectUrl%%", projectUrl)
                            .Replace("%%Message%%", emailTemplate.Message)
                            .Replace("%%ProjectDetailsTable%%", projectDetailsTable.ToString());

                foreach (var collabId in projectCollaboratorIds)
                {
                    if (collabId != requestedBy.Id)
                    {
                        var collaborator = allUsers.FirstOrDefault(x => x.Id == collabId);
                        if (collaborator != null)
                        {
                            var finalBody = body.Replace("%%To%%", collaborator.UserName);
                            var newEmailQueue = new EmailQueue()
                            {
                                Subject = emailTemplate.Subject,
                                Contents = finalBody,
                                ToAddress = collaborator.Email,
                                FromAddress = _configuration["DefaultEmail"],
                                CreatedById = requestedBy.Id,
                                Processed = false,
                                Error = false,
                                ErrorMessage = string.Empty,
                                RetryCount = 0,
                                EmailTemplateKey = EmailTemplateKeys.ProjectUpdatedCollaborator,
                                ProjectId = projectId
                            };

                            AddToEmailQueue(newEmailQueue);

                            // Temp until email server is configured
                            SendEmail(_configuration, emailTemplate.Subject, collaborator.Email, finalBody);
                        }
                    }
                };
            }
        }


        public void SaveProjectCommentToEmailQueue(List<string> taggedUserNames, string userId, int projectId, string comment)
        {
            var emailTemplate = _dbcontext.EmailTemplates.FirstOrDefault(a => a.EmailTemplateKey == EmailTemplateKeys.ProjectCommentTagged);
            var allUsers = _dbcontext.AppUsers.Where(x => !x.Deleted && x.Active).ToList();
            var project = _dbcontext.Projects.FirstOrDefault(x => x.Id == projectId);
            var requestedBy = allUsers.FirstOrDefault(x => x.Id == userId);

            if (emailTemplate != null)
            {
                var baseUrl = _configuration["BaseUrl"];
                var projectUrl = baseUrl + "projects/" + project.Id;

                var projectTable = new XElement("table");
                projectTable.Add(new XAttribute("cellpadding", 12));
                projectTable.Add(new XAttribute("border", 1));
                projectTable.Add(new XAttribute("style", "border-collapse:collapse"));

                var headers = new XElement("tr");
                headers.Add(new XElement("th", new XElement("span", "Project Name")));
                headers.Add(new XElement("th", new XElement("span", "Created By")));
                headers.Add(new XElement("th", new XElement("span", "Comment")));

                var projectDetails = new XElement("tr");
                projectDetails.Add(new XElement("td",
                                    new XElement("span",
                                        new XElement("a",
                                            new XAttribute("href",
                                                projectUrl),
                                            project.ProjectName
                                        )
                                    )
                                  )
                                );

                projectDetails.Add(new XElement("td", new XElement("span", requestedBy.UserName)));
                projectDetails.Add(new XElement("td", new XElement("span", comment)));

                projectTable.Add(headers);
                projectTable.Add(projectDetails);

                var body = emailTemplate.Contents
                            .Replace("%%EmailHeader%%", GetEmailHeaderUrl(emailTemplate.Classification))
                            .Replace("%%ProjectName%%", project.ProjectName)
                            .Replace("%%ProjectUrl%%", projectUrl)
                            .Replace("%%Message%%", emailTemplate.Message)
                            .Replace("%%ProjectTable%%", projectTable.ToString());

                foreach (var taggedUserName in taggedUserNames)
                {
                    var taggedUser = allUsers.FirstOrDefault(x => x.UserName == taggedUserName);
                    if (taggedUser.Id != requestedBy.Id)
                    {
                        var finalBody = body.Replace("%%To%%", taggedUser.UserName);
                        var newEmailQueue = new EmailQueue()
                        {
                            Subject = emailTemplate.Subject,
                            Contents = finalBody,
                            ToAddress = taggedUser.Email,
                            FromAddress = _configuration["DefaultEmail"],
                            CreatedById = requestedBy.Id,
                            Processed = false,
                            Error = false,
                            ErrorMessage = string.Empty,
                            RetryCount = 0,
                            EmailTemplateKey = EmailTemplateKeys.ProjectCommentTagged,
                            ProjectId = project.Id
                        };

                        AddToEmailQueue(newEmailQueue);

                        // Temp until email server is configured
                        SendEmail(_configuration, emailTemplate.Subject, taggedUser.Email, finalBody);
                    }
                };
            }
        }
    }
}
