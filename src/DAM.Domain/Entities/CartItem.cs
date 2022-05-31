using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using DAM.Domain.Entities.Identity;

namespace DAM.Domain.Entities
{
    public class CartItem
    {
        [Key]
        public int Id { get; set; }

        public int CartID { get; set; }
        public Cart Cart { get; set; }

        public int AssetID { get; set; }
        public Asset Asset { get; set; }
    }
}