using DAM.Domain.Entities.Identity;

namespace DAM.Domain.Entities
{
    public class TeamMember : EntityBase
    {
        public string UserId { get; set; }
        public ApplicationUser User { get; set; }

        public int TeamId { get; set; }
        public Team Team { get; set; }
    }
}
