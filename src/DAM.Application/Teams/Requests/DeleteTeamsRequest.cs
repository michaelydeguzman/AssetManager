using DAM.Application.Teams.Dtos;
using MediatR;
using System.Collections.Generic;

namespace DAM.Application.Teams.Requests
{
    public class DeleteTeamsRequest : IRequest<HandlerResult<DeleteTeamsDto>>
    {
        public DeleteTeamsDto Teams { get; set; }
        public string UserId { get; set; }
        public DeleteTeamsRequest(DeleteTeamsDto teams, string userId)
        {
            Teams = teams;
            UserId = userId;
        }
    }
}
