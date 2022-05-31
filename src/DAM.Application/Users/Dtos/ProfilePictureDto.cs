using System;
using System.Collections.Generic;
using System.Text;

namespace DAM.Application.Users.Dtos
{
    public class ProfilePictureDto
    {
        public byte[] FileBytes { get; set; }
        public string ImageFileExtension { get; set; }
        public string ImageUrl { get; set; }
    }
}
