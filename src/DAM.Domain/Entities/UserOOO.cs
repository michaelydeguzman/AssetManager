using DAM.Domain.Entities.Identity;
using System;
using System.Collections.Generic;
using System.Text;

namespace DAM.Domain.Entities
{
    public class UserOOO : EntityBase
    {
        public string UserId { get; set; }
        public ApplicationUser User { get; set; }
        public string Title { get; set; }
        public DateTimeOffset StartDate { get; set; }
        public DateTimeOffset EndDate { get; set; }
        public string Description { get; set; }
        public bool Deleted { get; set; }
        public string DefaultDelegateUser { get; set; }
    }
}
