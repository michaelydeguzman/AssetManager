using System;
using System.Collections.Generic;
using System.Text;

namespace DAM.Application.AuditTrail.Dtos
{
    public class AssetAuditDetailDto
    {
        public string DisplayName { get; set; }
        public string Description { get; set; }
        public List<string> Tags { get; set; }
        public List<string> Accounts { get; set; }
        public List<string> Countries { get; set; }
        public List<string> Regions { get; set; }
        public string Folder { get; set; }
        public DateTimeOffset? Expiry { get; set; }
        public string ShareFolders { get; set; }
        public string Status { get; set; }
    }
}
