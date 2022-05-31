using DAM.Application.Teams.Dtos;
using MediatR;
using System.Collections.Generic;

namespace DAM.Application.Teams.Requests
{
    public class TeamsRequest : IRequest<HandlerResult<IEnumerable<TeamDto>>>
    {
        public int TeamId { get; set; }
        public TeamsRequest(int teamId)
        {
            TeamId = teamId;
        }
    }
}
