using DAM.Application.Projects.Dtos;
using DAM.Application.Users.Dtos;
using DAM.Domain.Entities;
using DAM.Domain.Entities.Identity;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Text;

namespace DAM.Application.Services.Interfaces
{
    public interface IEmailService
    {
        class EmailServiceConfig
        {
            public string SMTPHost { get; set; }
            public int SMTPPort { get; set; }
            public string SMTPUsername { get; set; }
            public string SMTPPassword { get; set; }
            public string DefaultEmail { get; set; }
        }

        void AddToEmailQueue(EmailQueue emailQueue);

        void SendEmail(IConfiguration configuration, string subject, string to, string body);

        //void SendApproverEmails(ApprovalLevel level, List<ApprovalLevelApprover> approvers, string userId);

        //void SendReviewEmail(Asset asset, AssetVersions assetVersion, string approverId, string userId, bool isApproved, string comments);

        //void SendDelegateEmail(ApprovalLevel level, string newApproverId, string oldApproverId);

        //void SendForgotPasswordEmail(ApplicationUser user, string token);

        //void SendConfirmationEmail(ApplicationUser user, string confirmationLink);

        //void SendStatusChangeEmail(UpdateUserDto user);

        string GetEmailHeaderUrl(int emailClassification);

        void SaveEmailVerificationToEmailQueue(ApplicationUser invitee, ApplicationUser inviter, string confirmationLink);

        void SaveForgotPasswordToEmailQueue(ApplicationUser user, string token);

        void SaveApprovalEmailToEmailQueue(ApprovalLevel level, List<ApprovalLevelApprover> approvers, string userId, bool isProject, int sourceId, string sourceName);

        void SaveApprovedEmailToEmailQueue(Asset asset, AssetVersions assetVersion, string approverId, string userId, string comments, bool isProject, int sourceId, string sourceName);

        void SaveRejectedEmailToEmailQueue(Asset asset, AssetVersions assetVersion, string approverId, string userId, string comments, bool isProject, int sourceId, string sourceName);

        void SaveDelegateApprovalEmailToEmailQueue(ApprovalLevel level, string newApproverId, string oldApproverId);

        void SaveAddedOwnersToEmailQueue(List<string> newOwnerIds, int projectId, string projectName, string userId);

        void SaveAddedCollaboratorsToEmailQueue(List<string> newCollaboratorIds, int projectId, string projectName, string userId);

        void SaveProjectDeletedOwnerToEmailQueue(List<string> projectOwnerIds, int projectId, string projectName, string userId);

        void SaveProjectDeletedCollaboratorToEmailQueue(List<string> projectCollaboratorIds, int projectId, string projectName, string userId);

        void SaveProjectArchivedOwnerToEmailQueue(List<string> projectOwnerIds, int projectId, string projectName, string userId);

        void SaveProjectArchivedCollaboratorToEmailQueue(List<string> projectCollaboratorIds, int projectId, string projectName, string userId);

        void SaveProjectUpdatedOwnerToEmailQueue(List<string> projectOwnerIds, List<ProjectUpdateDetailsDto> details, int projectId, string projectName, string userId);

        void SaveProjectUpdatedCollaboratorToEmailQueue(List<string> projectCollaboratorIds, List<ProjectUpdateDetailsDto> details, int projectId, string projectName, string userId);

        void SaveProjectCommentToEmailQueue(List<string> taggedUserNames, string userId, int projectId, string comment);
    }
}
