using AutoMapper;
using DAM.Application.Cache;
using DAM.Persistence;
using System;
using System.Collections.Generic;
using System.Threading;
using DAM.Application.Logos.Dtos;
using DAM.Application.Logos.Requests;
using DAM.Application.Styles.Dtos;

namespace DAM.Application.Logos.Handlers
{
    public class GetLogosRequestHandler : HandlerBase<GetLogosRequest, HandlerResult<IEnumerable<LogoDto>>>
    {
        private readonly IDbContext _dbcontext;
        private readonly IMapper _mapper;

        public GetLogosRequestHandler(ICacheProvider cacheProvider, IDbContext dbcontext, IMapper mapper)
        {
            _dbcontext = dbcontext ?? throw new ArgumentNullException(nameof(dbcontext));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        public override HandlerResult<IEnumerable<LogoDto>> HandleRequest(GetLogosRequest request, CancellationToken cancellationToken, HandlerResult<IEnumerable<LogoDto>> result)
        {
            var logos = _dbcontext.Logos;

            result.Entity = _mapper.Map<List<LogoDto>>(logos);

            result.ResultType = ResultType.Success;
            return result;
        }

        public override Exception OnException(Exception ex)
        {
            return new Exception(ex.Message, ex);
        }
    }
}