using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace DAM.Domain.Entities
{
    public class Logo: EntityBase
    {
        [Key]
        public int? Id { get; set; }
        public string FileName { get; set; }

        public string LogoUrl { get; set; }

        public bool IsApplied { get; set; }

        public bool IsDeleted { get; set; }
    }
}
