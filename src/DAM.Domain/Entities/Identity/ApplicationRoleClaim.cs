using Microsoft.AspNetCore.Identity;

namespace DAM.Domain.Entities.Identity
{
    public class ApplicationRoleClaim : IdentityRoleClaim<string>
    {
        public virtual ApplicationRole Role { get; set; }
    }
}
