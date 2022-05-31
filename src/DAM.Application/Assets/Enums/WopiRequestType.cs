using System;
using System.Collections.Generic;
using System.Text;

namespace DAM.Application.Assets.Enums
{
    public enum WopiRequestType
    {
        None,
        CheckFileInfo,
        GetFile,
        Lock,
        GetLock,
        RefreshLock,
        Unlock,
        UnlockAndRelock,
        PutFile,
        PutRelativeFile,
        RenameFile,
        PutUserInfo

        /*
        DeleteFile, //ONENOTE ONLY        
        ExecuteCellStorageRequest, //ONENOTE ONLY
        ExecuteCellStorageRelativeRequest, //ONENOTE ONLY
        ReadSecureStore, //NO DOCS
        GetRestrictedLink, //NO DOCS
        RevokeRestrictedLink, //NO DOCS
        ExecuteCobaltRequest, //In GitHub Sample
        CheckFolderInfo, //In GitHub Sample
        EnumerateChildren //In GitHub Sample
        */
    }

}
