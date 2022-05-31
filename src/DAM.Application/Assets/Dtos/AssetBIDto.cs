using System;
using System.Collections.Generic;
using DAM.Application.Companies.Dtos;

namespace DAM.Application.Assets.Dtos
{
    public class AssetBIDto
    {
        public int Id {get; set;}
        public string Title {get; set;}
        public string FileName {get; set;}
        public int? FolderId {get; set;}
        public int DownloadCount {get; set;}
        public string CompanyName {get; set;}
        public int? CompanyId {get; set;}
        public List<DateTimeOffset> AuditTracking {get; set;}
    }
}