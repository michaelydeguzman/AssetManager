using System;
using System.Collections.Generic;
using System.Text;

namespace DAM.Application.Logos.Dtos
{
    public class LogoDto
    {
        public int? Id { get; set; }
        public string FileName { get; set; }

        public string LogoUrl { get; set; }

        public bool IsApplied { get; set; }

        public bool IsDeleted { get; set; }

        public byte[] Logo { get; set; }
    }
}
