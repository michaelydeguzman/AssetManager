using DAM.Application.Teams.Dtos;
using MediatR;

namespace DAM.Application.Teams.Requests
{
    public class SaveTeamRequest : IRequest<HandlerResult<TeamDto>>
    {
        public string UserId { get; set; }

        public TeamDto Team { get; set; }

        public SaveTeamRequest(TeamDto team, string userId)
        {
            UserId = userId;
            Team = team;
        }
    }
}
