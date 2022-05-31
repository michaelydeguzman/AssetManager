using System;
using System.Collections.Generic;
using System.Text;

namespace DAM.Application.Companies.Dtos
{
    public class CompanyDto
    {
        public int Id { get; set; }
        public string CompanyName { get; set; }
        public bool Status { get; set; }
        public int? RootFolderId { get; set; }
        public string RootFolderName { get; set; }
        public string CreatedById { get; set; }
        public DateTimeOffset? CreatedDate { get; set; }
        public string ModifiedById { get; set; }
        public DateTimeOffset? ModifiedDate { get; set; }
        public bool UpdateUsers { get; set; }
    }
}
