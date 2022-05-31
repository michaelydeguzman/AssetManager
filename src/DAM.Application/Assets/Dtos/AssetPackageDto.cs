using System;
using System.Collections.Generic;
using DAM.Application.Companies.Dtos;

namespace DAM.Application.Assets.Dtos
{
    public class AssetPackageDto
    {
        public int AssetId {get; set;}
        public byte[] Package { get; set; }
        public string Extension { get; set; }
        public string FileType { get; set; }
        public string PackageName { get; set; }
    }
}