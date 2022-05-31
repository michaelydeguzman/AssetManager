using DAM.Domain.Entities.Identity;
using System;
using System.Collections.Generic;
using System.Text;

namespace DAM.Domain.Entities
{
    public class Watermark : EntityBase
    {
        public string WatermarkUrl { get; set; }
        public int? CompanyId { get; set; }
        public int? FolderId { get; set; }
        public decimal Opacity { get; set; }
        public decimal Size { get; set; }
        public int WatermarkPosition { get; set; }
    }
}
