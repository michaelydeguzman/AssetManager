using System;
using System.Collections.Generic;
using System.Text;

namespace DAM.Application.AuditTrail.Dtos
{
    public class FetchAuditDto
    {
        public int CurrentPageNumber { get; set; }

        public string SortColumnName { get; set; }

        public int SortOrder { get; set; }

        public int PageSize { get; set; }

        public int? AuditType { get; set; }
        public string FileName{get; set;}
        public string FolderName{get; set;}
    }
}
