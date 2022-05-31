using System;
using System.Collections.Generic;
using System.Text;

namespace DAM.Application.AuditTrail.Dtos
{
    public class FolderAuditDetailDto
    {
        public string DisplayName { get; set; }
        public string Description { get; set; }
        public List<string> Accounts { get; set; }
        public List<string> Countries { get; set; }
        public List<string> Regions { get; set; }
        public string Folder { get; set; }
    }
}
