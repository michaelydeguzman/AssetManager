using AutoMapper;
using DAM.Application.Cache;
using DAM.Persistence;
using System;
using System.Collections.Generic;
using System.Threading;
using DAM.Application.Teams.Dtos;
using DAM.Application.Teams.Requests;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace DAM.Application.Teams.Handlers
{
    public class DeleteTeamsRequestHandler : HandlerBase<DeleteTeamsRequest, HandlerResult<DeleteTeamsDto>>
    {
        private readonly IDbContext _dbcontext;
        private readonly IMapper _mapper;

        public DeleteTeamsRequestHandler(ICacheProvider cacheProvider, IDbContext dbcontext, IMapper mapper)
        {
            _dbcontext = dbcontext ?? throw new ArgumentNullException(nameof(dbcontext));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        public override HandlerResult<DeleteTeamsDto> HandleRequest(DeleteTeamsRequest request, CancellationToken cancellationToken, HandlerResult<DeleteTeamsDto> result)
        {
            var teamsToDelete = _dbcontext.Teams.Where(t => request.Teams.TeamIds.Contains(t.Id));
            var allProjectTeams = _dbcontext.ProjectTeamFollowers.ToList();

            foreach(var team in teamsToDelete)
            {
                team.Deleted = true;
                team.ModifiedById = request.UserId;
                team.ModifiedDate = DateTimeOffset.UtcNow;

                var projectTeams = allProjectTeams.Where(pt => pt.TeamId == team.Id);

                _dbcontext.ProjectTeamFollowers.RemoveRange(projectTeams);
                _dbcontext.SaveChanges();
            }
            _dbcontext.Teams.UpdateRange(teamsToDelete);
            _dbcontext.SaveChanges();

            result.Entity = request.Teams;
            result.ResultType = ResultType.Success;
            return result;
        }

        public override Exception OnException(Exception ex)
        {
            return new Exception(ex.Message, ex);
        }
    }
}