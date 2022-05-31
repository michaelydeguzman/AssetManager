using DAM.Domain.Entities.Identity;
using System;
using System.Collections.Generic;
using System.Text;

namespace DAM.Application.Watermarks.Dto
{
    public class WatermarkDto
    {
        public int? Id { get; set; }
        public string WatermarkUrl { get; set; }
        public int? CompanyId { get; set; }
        public int? FolderId { get; set; }
        public decimal Opacity { get; set; }
        public decimal Size { get; set; }
        public int WatermarkPosition { get; set; }

        public byte[] Watermark { get; set; }
    }
}
