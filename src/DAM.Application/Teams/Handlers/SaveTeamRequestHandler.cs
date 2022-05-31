using AutoMapper;
using DAM.Application.Cache;
using DAM.Persistence;
using System;
using System.Linq;
using System.Threading;
using Microsoft.Extensions.Configuration;
using DAM.Domain.Entities;
using DAM.Application.Teams.Dtos;
using DAM.Application.Teams.Requests;
using System.Collections.Generic;

namespace DAM.Application.Teams.Handlers
{
    public class SaveTeamRequestHandler : HandlerBase<SaveTeamRequest, HandlerResult<TeamDto>>
    {
        private readonly IDbContext _dbcontext;
        private readonly IMapper _mapper;
        private readonly IConfiguration _configuration;

        public SaveTeamRequestHandler(ICacheProvider cacheProvider, IDbContext dbcontext, IMapper mapper, IConfiguration configuration)
        {
            _dbcontext = dbcontext ?? throw new ArgumentNullException(nameof(dbcontext));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
        }

        public override HandlerResult<TeamDto> HandleRequest(SaveTeamRequest request, CancellationToken cancellationToken, HandlerResult<TeamDto> result)
        {
            result.Entity = new TeamDto();
            var team = new Team();

            if (request.Team.Id.HasValue)
            {
                team = _dbcontext.Teams.FirstOrDefault(x => x.Id == request.Team.Id.Value);

                if (team == null)
                {
                    result.ResultType = ResultType.NoData;
                    return result;
                }
                else
                {
                    team.TeamName = request.Team.TeamName;
                    team.TeamDescription = request.Team.TeamDescription;
                    team.Approvals = request.Team.Approvals;
                    team.Project = request.Team.Project;
                    team.Status = request.Team.Status;
                    team.ModifiedById = request.UserId;
                    team.ModifiedDate = DateTimeOffset.UtcNow;
                    _dbcontext.Teams.Update(team);
                }
            }
            else
            {
                //save new Team
               
                team.TeamName = request.Team.TeamName;
                team.TeamDescription = request.Team.TeamDescription;
                team.Approvals = request.Team.Approvals;
                team.Project = request.Team.Project;
                team.Status = request.Team.Status;
                team.CreatedById = request.UserId;
                team.Deleted = false;

                _dbcontext.Teams.Add(team);
            }

            _dbcontext.SaveChanges();


            // process team members

            var teamMemberUserIds = request.Team.TeamMemberUserIds;
            var allTeamMembers = _dbcontext.TeamMembers.Where(tm => tm.TeamId == team.Id).ToList();
            var existingMembersId = allTeamMembers.Select(x => x.UserId);

            var teamMembersToAdd = new List<TeamMember>();

            foreach (var userId in teamMemberUserIds)
            {
                if (!allTeamMembers.Any(tm => tm.UserId == userId))
                {
                    var newTeamMember = new TeamMember()
                    {
                        TeamId = team.Id,
                        UserId = userId,
                        CreatedById = request.UserId
                    };
                    teamMembersToAdd.Add(newTeamMember);
                }
            }

            _dbcontext.TeamMembers.AddRange(teamMembersToAdd);
            _dbcontext.SaveChanges();

            var teamMembersToDelete = new List<TeamMember>();

            foreach (var existingMember in allTeamMembers)
            {
                if (!teamMemberUserIds.Contains(existingMember.UserId))
                {
                    teamMembersToDelete.Add(existingMember);
                }
            }

            _dbcontext.TeamMembers.RemoveRange(teamMembersToDelete);
            _dbcontext.SaveChanges();

            result.Entity = _mapper.Map<TeamDto>(team);
            result.ResultType = ResultType.Success;

            return result;
        }

        public override Exception OnException(Exception ex)
        {
            return new Exception(ex.Message, ex);
        }
    }
}