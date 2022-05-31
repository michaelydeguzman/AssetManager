using System;
using System.Threading;
using AutoMapper;
using DAM.Persistence;
using System.Linq;
using DAM.Application.Projects.Requests;
using DAM.Application.Projects.Dtos;
using DAM.Domain.Entities;
using System.Collections.Generic;
using DAM.Application.Projects.Enums;

namespace DAM.Application.Projects.Handlers
{
    public class UnarchiveProjectRequestHandler : HandlerBase<UnarchiveProjectRequest, HandlerResult<ProjectDto>>
    {
        private readonly IDbContext _dbcontext;
        private readonly IMapper _mapper;

        public UnarchiveProjectRequestHandler(IMapper mapper, IDbContext dbcontext)
        {
            _dbcontext = dbcontext ?? throw new ArgumentNullException(nameof(dbcontext));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        public override HandlerResult<ProjectDto> HandleRequest(UnarchiveProjectRequest request, CancellationToken cancellationToken, HandlerResult<ProjectDto> result)
        {
            var updateProject = _dbcontext.Projects.FirstOrDefault(p => p.Id == request.ProjectId);

            updateProject.ProjectStatus = updateProject.StatusBeforeArchive;
            updateProject.ModifiedDate = DateTimeOffset.UtcNow;
            updateProject.ModifiedById = request.UserId;

            _dbcontext.Projects.Update(updateProject);
            _dbcontext.SaveChanges();

            result.Entity = _mapper.Map<ProjectDto>(updateProject);

            _dbcontext.SaveChanges();

            result.ResultType = ResultType.Success;
            return result;
        }
    }
}
