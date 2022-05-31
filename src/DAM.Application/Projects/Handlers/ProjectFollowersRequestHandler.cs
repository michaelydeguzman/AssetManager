using System;
using System.Threading;
using AutoMapper;
using DAM.Persistence;
using System.Collections.Generic;
using System.Linq;
using DAM.Application.Projects.Requests;
using DAM.Application.Projects.Dtos;
using DAM.Application.Teams.Enums;

namespace DAM.Application.Projects.Handlers
{
    public class ProjectFollowersRequestHandler : HandlerBase<ProjectFollowersRequest, HandlerResult<ProjectFollowersDto>>
    {
        private readonly IDbContext _dbcontext;
        private readonly IMapper _mapper;

        public ProjectFollowersRequestHandler(IMapper mapper, IDbContext dbcontext)
        {
            _dbcontext = dbcontext ?? throw new ArgumentNullException(nameof(dbcontext));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        public override HandlerResult<ProjectFollowersDto> HandleRequest(ProjectFollowersRequest request, CancellationToken cancellationToken, HandlerResult<ProjectFollowersDto> result)
        {
            var projectUserFollowers = _dbcontext.ProjectUserFollowers.Where(p => p.ProjectId == request.ProjectId);
            var projectTeamFollowers = _dbcontext.ProjectTeamFollowers.Where(t => t.ProjectId == request.ProjectId).ToList();
            var allActiveTeams = _dbcontext.Teams.Where(t => t.Status == (int)TeamStatus.Active && !t.Deleted).ToList();

            var activeProjectTeams = (from ptf in projectTeamFollowers
                                      join t in allActiveTeams
                                      on ptf.TeamId equals t.Id
                                      select ptf).ToList();

            result.Entity = new ProjectFollowersDto();

            result.Entity.ProjectId = request.ProjectId;
            result.Entity.Users = _mapper.Map<List<ProjectUserFollowerDto>>(projectUserFollowers);
            result.Entity.Teams = _mapper.Map<List<ProjectTeamFollowerDto>>(activeProjectTeams);

            result.ResultType = ResultType.Success;
            return result;
        }
    }
}
