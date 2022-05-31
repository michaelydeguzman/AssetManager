using DAM.Domain.Entities.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace DAM.Domain.Entities
{
    public class DAMToDynamic
    {
        [Key]
        public int Id { get; set; }
        public int AssetId { get; set; }
        public ApplicationUser UploadedBy { get; set; }
        public bool IsUploaded { get; set; }
        public bool IsMarketing { get; set; }
        public string Entity { get; set; }
        public string EntityId { get; set; }
    }
}
