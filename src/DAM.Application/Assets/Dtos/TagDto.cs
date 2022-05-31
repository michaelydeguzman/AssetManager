using System;
using System.Collections.Generic;
using System.Text;

namespace DAM.Application.Assets.Dtos
{
    public class TagDto
    {
        public int? Id { get; set; }
        //public int AssetId { get; set; }
        public string Name { get; set; }
        public bool IsCognitive { get; set; }
    }
}
