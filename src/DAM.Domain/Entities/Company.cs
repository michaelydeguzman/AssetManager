using DAM.Domain.Entities.Identity;
using System;
using System.Collections.Generic;
using System.Text;

namespace DAM.Domain.Entities
{
    public class Company : EntityBase
    {
        public string CompanyName { get; set; }
        public bool Status { get; set; }
        public int? RootFolderId { get; set; }
        public Folder RootFolder { get; set; }
    }
}
