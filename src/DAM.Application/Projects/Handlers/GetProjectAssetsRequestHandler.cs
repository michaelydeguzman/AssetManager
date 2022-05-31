using System;
using System.Threading;
using AutoMapper;
using DAM.Application.Carts.Dtos;
using DAM.Application.Carts.Requests;
using DAM.Persistence;
using DAM.Domain.Entities;
using System.Collections.Generic;
using DAM.Application.Assets.Dtos;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using DAM.Application.Projects.Requests;
using DAM.Application.Projects.Dtos;
using DAM.Application.Projects.Enums;

namespace DAM.Application.Projects.Handlers
{
    public class GetProjectAssetsRequestHandler : HandlerBase<GetProjectAssetsRequest, HandlerResult<ProjectDto>>
    {
        private readonly IDbContext _dbcontext;
        private readonly IMapper _mapper;

        public GetProjectAssetsRequestHandler(IMapper mapper, IDbContext dbcontext)
        {
            _dbcontext = dbcontext ?? throw new ArgumentNullException(nameof(dbcontext));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        public override HandlerResult<ProjectDto> HandleRequest(GetProjectAssetsRequest request, CancellationToken cancellationToken, HandlerResult<ProjectDto> result)
        {

            //var project = _dbcontext.Projects.Where(p => p.Id == request.ProjectId);

            //{
            //    var projectItems = _dbcontext.ProjectItems.Where(pi => pi.ProjectId == project.Id.Value).ToList();
            //    project.ProjectItems = _mapper.Map<List<ProjectItemDto>>(projectItems);

            //    project.ProjectUploads = _dbcontext.Assets.Where(a => a.ProjectId == project.Id.Value).Select(a => a.Id).ToList();
            //}

            result.ResultType = ResultType.Success;
            return result;
        }
    }
}
