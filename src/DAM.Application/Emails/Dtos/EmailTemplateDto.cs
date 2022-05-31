using System;
using System.Collections.Generic;
using System.Text;

namespace DAM.Application.Emails.Dtos
{
    public class EmailTemplateDto
    {
        public int Id { get; set; }
        public string EmailTemplateName { get; set; }
        public string EmailTemplateKey { get; set; }
        public string Subject { get; set; }
        public string Contents { get; set; }
        public string Message { get; set; }
        public bool Deleted { get; set; }
        public string RecipientType { get; set; }
        public int Category { get; set; }
        public int Classification { get; set; }
    }
}
