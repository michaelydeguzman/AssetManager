using DAM.Domain.Entities.Identity;

namespace DAM.Domain.Entities
{
    public class ProjectTeamFollower : EntityBase
    {
        public int TeamId { get; set; }
        public Team Team {get; set;}

        public int ProjectId { get; set; }
        public Project Project { get; set; }
    }
}
