using System;
using System.Collections.Generic;
using System.Text;

namespace DAM.Application.Users.Constants
{
    public static class EmailTemplateKeys
    {
        public const string UserVerificationEmail = "USER_VERIFICATION";
        public const string PasswordResetEmail = "PASSWORD_RESET";
        public const string AssetSentForApprovalEmail = "SENT_FOR_APPROVAL";
        public const string AssetResentForApprovalEmail = "RESENT_FOR_APPROVAL";
        public const string AssetDelegatedForApprovalEmail = "DELEGATE_APPROVAL";
        public const string AssetApproved = "ASSET_APPROVED";
        public const string AssetRejected = "ASSET_REJECTED";
        public const string ProjectDeletedOwner = "PROJECT_DELETED_OWNER";
        public const string ProjectDeletedCollaborator = "PROJECT_DELETED_COLLABORATOR";
        public const string ProjectArchivedOwner = "PROJECT_ARCHIVED_OWNER";
        public const string ProjectArchivedCollaborator = "PROJECT_ARCHIVED_COLLABORATOR";
        public const string ProjectOwnerAdded = "PROJECT_OWNER_ADDED";
        public const string ProjectCollaboratorAdded = "PROJECT_COLLABORATOR_ADDED";
        public const string ProjectUpdatedOwner = "PROJECT_UPDATED_OWNER";
        public const string ProjectUpdatedCollaborator = "PROJECT_UPDATED_COLLABORATOR";
        public const string ApprovalOverdue = "APPROVAL_OVERDUE";
        public const string ApprovalOverdueSubmitter = "APPROVAL_OVERDUE_SUBMITTER";
        public const string ProjectCommentTagged = "PROJECT_COMMENT_TAGGED";
    }
}
