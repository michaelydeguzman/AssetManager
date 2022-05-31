using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace DAM.Domain.Entities
{
    public class EmailQueue : EntityBase
    {
        public string EmailTemplateKey { get; set; }

        public int? ProjectId { get; set; }

        public int? FolderId { get; set; }

        public int? AssetId { get; set; }

        public int? AssetVersionId { get; set; }

        public string Subject { get; set; }

        public string Contents { get; set; }

        public string ToAddress { get; set; }

        public string FromAddress { get; set; }

        public bool Processed { get; set; }

        public int RetryCount { get; set; }

        public bool Error { get; set; }

        public string ErrorMessage { get; set; }
    }
}
