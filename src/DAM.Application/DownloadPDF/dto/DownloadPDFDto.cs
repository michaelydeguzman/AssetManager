using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace DAM.Application.DownloadPDF.dto
{
    public class DownloadPDFDto
    {
        public byte[] AssetStream { get; set; }
        public string Annotation { get; set; }
        public string FileName { get; set; }
        public Stream OutputStream { get; set; }
        public string ContentType { get; set; }
        public int Rotation { get; set; }
    }
}