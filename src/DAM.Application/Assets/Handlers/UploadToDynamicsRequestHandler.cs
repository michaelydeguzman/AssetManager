using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using AutoMapper;
using DAM.Application.Assets.Dtos;
using DAM.Application.Assets.Requests;
using DAM.Application.Cache;
using DAM.Domain.Entities;
using DAM.Persistence;
using Microsoft.Extensions.Configuration;
using System.Linq;

namespace DAM.Application.Assets.Handlers
{
    public class UploadToDynamicsRequestHandler : HandlerBase<UploadToDynamicsRequest, HandlerResult<List<UploadToDynamicsDto>>>
    {
        private readonly ICacheProvider _cacheProvider;
        private readonly IDbContext _dbcontext;
        private readonly IMapper _mapper;
        private readonly IConfiguration _configuration;

        public UploadToDynamicsRequestHandler(ICacheProvider cacheProvider, IDbContext dbcontext, IMapper mapper, IConfiguration configuration)
        {
            _cacheProvider = cacheProvider ?? throw new ArgumentNullException(nameof(cacheProvider));
            _dbcontext = dbcontext ?? throw new ArgumentNullException(nameof(dbcontext));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
        }

        public override HandlerResult<List<UploadToDynamicsDto>> HandleRequest(UploadToDynamicsRequest request, CancellationToken cancellationToken, HandlerResult<List<UploadToDynamicsDto>> result)
        {
            result.Entity = new List<UploadToDynamicsDto>();

            foreach (var req in request.UploadToDynamics)
            {
                req.UploadedByUser = _dbcontext.AppUsers.First(u => u.Id == req.UploadedBy);
                var damToDynamics = _mapper.Map<DAMToDynamic>(req);

                _dbcontext.DAMToDynamics.Add(damToDynamics);
                _dbcontext.SaveChanges();

                result.Entity.Add(_mapper.Map<UploadToDynamicsDto>(damToDynamics));

                //var upload = _dbcontext.DAMToDynamics.First(d => d.Id == damToDynamic.Id);

                //while (!upload.IsUploaded)
                //{
                //    upload = _dbcontext.DAMToDynamics.First(d => d.Id == damToDynamic.Id);

                //    req.IsUploadDone = upload.IsUploaded;
                //}
            }
            result.ResultType = ResultType.Success;
            return result;
        }

        public override Exception OnException(Exception ex)
        {
            return new Exception(ex.Message, ex);
        }
    }
}
