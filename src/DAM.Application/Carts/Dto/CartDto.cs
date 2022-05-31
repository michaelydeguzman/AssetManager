using System;
using DAM.Domain.Entities.Identity;
using System.Collections.Generic;
using DAM.Domain.Entities;
using DAM.Application.Assets.Dtos;

namespace DAM.Application.Carts.Dtos
{
    public class CartDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public String UserId { get; set; }  
        public List<int> AssetIds { get; set; }
        public Boolean IsCurrentCart { get; set; }
    }
}
