using DAM.Application.Accounts.Dtos;
using DAM.Application.Assets.Dtos;
using System;
using System.Collections.Generic;
using System.Text;

namespace DAM.Application.Assets
{
    public class AssetAccountMetaDataDto
    {
        public int? Id { get; set; }

        public int AssetId { get; set; }
        public AssetDto Asset { get; set; }
        public int AccountId { get; set; }
        public AccountDto Account { get; set; }

        public string CreatedById { get; set; }
        public DateTimeOffset? CreatedDate { get; set; }
        public string ModifiedById { get; set; }
        public DateTimeOffset? ModifiedDate { get; set; }
    }
}
