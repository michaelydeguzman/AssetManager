using DAM.Application.Assets;
using System;
using System.Collections.Generic;
using System.Text;

namespace DAM.Application.Accounts.Dtos
{
    public class AccountDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int AssetId { get; set; }

        //public List<AssetAccountMetaDataDto> AssetAccounts { get; set; }
        //public List<FolderAccountMetaDataDto> FolderAccounts { get; set; }
    }
}
