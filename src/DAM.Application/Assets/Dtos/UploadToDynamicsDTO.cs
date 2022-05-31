using System;
using System.Collections.Generic;
using System.Text;
using DAM.Application.Users.Dtos;
using DAM.Domain.Entities;
using DAM.Domain.Entities.Identity;

namespace DAM.Application.Assets.Dtos
{
    public class UploadToDynamicsDto
    {
        public int AssetId { get; set; }
        public string UploadedBy { get; set; }
        public string Entity { get; set; }
        public string EntityId { get; set; }
        public bool? IsUploadDone { get; set; }
        public bool? IsMarketing { get; set; }
        public ApplicationUser UploadedByUser { get; set; }
    }
}
