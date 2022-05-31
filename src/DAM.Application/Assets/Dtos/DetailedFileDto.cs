using System;
using System.Collections.Generic;
using System.Text;

namespace DAM.Application.Assets.Dtos
{
    public class DetailedFileDto : AssetDto
    {
        public string UserId { get; set; }

        public string CloseUrl { get; set; }

        public string HostEditUrl { get; set; }

        public string HostViewUrl { get; set; }

        public bool SupportsCoauth
        {
            get { return false; }
        }

        public bool SupportsExtendedLockLength
        {
            get { return false; }
        }

        public bool SupportsFileCreation
        {
            get { return false; }
        }

        public bool SupportsFolders
        {
            get { return false; }
        }

        public bool SupportsGetLock
        {
            get { return true; }
        }

        public bool SupportsLocks
        {
            get { return true; }
        }

        public bool SupportsRename
        {
            get { return true; }
        }

        public bool SupportsScenarioLinks
        {
            get { return false; }
        }

        public bool SupportsSecureStore
        {
            get { return false; }
        }

        public bool SupportsUpdate
        {
            get { return true; }
        }

        public bool SupportsUserInfo
        {
            get { return true; }
        }

        public bool LicensesCheckForEditIsEnabled
        {
            get { return true; }
        }

        /// <summary>
        /// Permissions for documents
        /// </summary>
        public bool ReadOnly
        {
            get { return false; }
        }

        public bool RestrictedWebViewOnly
        {
            get { return false; }
        }

        public bool UserCanAttend
        {
            get { return false; }
        }

        public bool UserCanNotWriteRelative
        {
            get { return false; }
        }

        public bool UserCanPresent
        {
            get { return false; }
        }

        public bool UserCanRename
        {
            get { return true; }
        }

        public bool UserCanWrite
        {
            get { return true; }
        }

        public bool WebEditingDisabled
        {
            get { return false; }
        }

        public List<WopiActionDto> Actions { get; set; }
    }
}
