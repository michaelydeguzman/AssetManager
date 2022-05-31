using System;
using System.Collections.Generic;
using DAM.Application.Companies.Dtos;

namespace DAM.Application.Assets.Dtos
{
    public class AssetThumbnailDto
    {
        public int AssetId {get; set;}
        public byte[] Thumbnail { get; set; }
        public string Extension { get; set; }
        public string FileType { get; set; }
    }
}