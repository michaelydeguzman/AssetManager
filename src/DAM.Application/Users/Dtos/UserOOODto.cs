using System;
using System.Collections.Generic;
using System.Text;

namespace DAM.Application.Users.Dtos
{
    public class UserOOODto 
    {
        public int? Id { get; set; }
        public string UserId { get; set; }
        public string Title { get; set; }
        public DateTimeOffset StartDate { get; set; }
        public DateTimeOffset EndDate { get; set; }
        public string Description { get; set; }
        public bool Deleted { get; set; }
        public string DefaultDelegateUser { get; set; }
    }
}
