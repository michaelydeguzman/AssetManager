using DAM.Application.Accounts.Dtos;
using DAM.Application.Assets.Dtos;
using DAM.Application.Folders.Dtos;
using System;
using System.Collections.Generic;
using System.Text;

namespace DAM.Application.Assets
{
    public class FolderAccountMetaDataDto
    {
        public int? Id { get; set; }

        public int FolderId { get; set; }
        public FolderDto Folder { get; set; }
        public int AccountId { get; set; }
        public AccountDto Account { get; set; }

        public string CreatedById { get; set; }
        public DateTimeOffset? CreatedDate { get; set; }
        public string ModifiedById { get; set; }
        public DateTimeOffset? ModifiedDate { get; set; }
    }
}
