using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace DAM.Application.Assets.Dtos
{
    public class DownloadDto
    {
        public Stream Content { get; set; }
        public string ContentType { get; set; }
        public string FileName { get; set; }
    }
}
