using System;
using System.Collections.Generic;
using System.Text;

namespace DAM.Application.Users.Dtos
{
    public class UserImageDto
    {
        public int UserId { get; set; }
        public string Extension { get; set; }
        public byte[] FileBytes { get; set; }
        public long Size { get; set; }
        public string FileSizeText { get; set; }
        public string FileType { get; set; }
        public string DownloadUrl { get; set; }
        public string Thumbnail { get; set; }
    }
}
