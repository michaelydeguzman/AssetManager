using System;
using System.Collections.Generic;
using System.Text;

namespace DAM.Application.Assets.Dtos
{
    public class ShareDto
    {
        public string EmailAddress { get; set; }
        public string AssetKey { get; set; }
        public bool ShowWaterMark { get; set; }
        public string Extension { get; set; }
    }
}
