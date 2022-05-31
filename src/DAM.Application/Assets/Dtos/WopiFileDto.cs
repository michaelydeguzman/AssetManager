using System;
using System.Diagnostics;
using System.Globalization;
using System.IO;

namespace DAM.Application.Assets.Dtos
{
    public class WopiFileDto
    {
        public string Id { get; set; }
        public string LockValue { get; set; }
        public DateTime? LockExpires { get; set; }
        public string OwnerId { get; set; }
        public string BaseFileName { get; set; }
        public string Container { get; set; }
        public long Size { get; set; }
        public int Version { get; set; }
        public string UserInfo { get; set; }
    }
}
