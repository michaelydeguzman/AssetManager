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
    public class TeamsRequestHandler : HandlerBase<TeamsRequest, HandlerResult<IEnumerable<TeamDto>>>
    {
        private readonly IDbContext _dbcontext;
        private readonly IMapper _mapper;

        public TeamsRequestHandler(ICacheProvider cacheProvider, IDbContext dbcontext, IMapper mapper)
        {
            _dbcontext = dbcontext ?? throw new ArgumentNullException(nameof(dbcontext));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        public override HandlerResult<IEnumerable<TeamDto>> HandleRequest(TeamsRequest request, CancellationToken cancellationToken, HandlerResult<IEnumerable<TeamDto>> result)
        {
            var teams = _dbcontext.Teams.Where(t=>!t.Deleted).Include(t=>t.TeamMembers);

            result.Entity = _mapper.Map<List<TeamDto>>(teams);

            result.ResultType = ResultType.Success;
            return result;
        }

        public override Exception OnException(Exception ex)
        {
            return new Exception(ex.Message, ex);
        }
    }
}