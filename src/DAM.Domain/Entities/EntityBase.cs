using DAM.Domain.Entities.Identity;
using System;
using System.Collections.Generic;
using System.Text;

namespace DAM.Domain.Entities
{
    public abstract class EntityBase
    {
        public int Id { get; set; }

        // Old
        //public string CreatedBy { get; set; }
        //public User CreatedByUser { get; set; }

        //public string ModifiedBy { get; set; }
        //public User ModifiedByUser { get; set; }

        // Identity
        public string CreatedById { get; set; }
        public ApplicationUser CreatedBy { get; set; }
        public string ModifiedById { get; set; }
        public ApplicationUser ModifiedBy { get; set; }

        public DateTimeOffset CreatedDate { get; private set; }
        public DateTimeOffset? ModifiedDate { get; set; }

        public EntityBase()
        {
            CreatedDate = DateTimeOffset.UtcNow;
        }
    }
}