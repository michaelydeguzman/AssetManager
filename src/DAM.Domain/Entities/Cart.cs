using System;
using DAM.Domain.Entities.Identity;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace DAM.Domain.Entities
{
    public class Cart
    {
        public int Id { get; set; }
        public string Name { get; set; }

        public Boolean IsCurrentCart { get; set; }

        public String UserId { get; set; }
        public ApplicationUser User { get; set; }

        public virtual ICollection<CartItem> CartItems { get; set; }
    }
}
