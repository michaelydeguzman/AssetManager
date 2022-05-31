using AutoMapper;
using DAM.Application.Cache;
using DAM.Persistence;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using DAM.Application.Styles.Requests;
using DAM.Application.Styles.Dtos;

namespace DAM.Application.Styles.Handlers
{
    public class GetStylesRequestHandler : HandlerBase<GetStylesRequest, HandlerResult<IEnumerable<ThemeDto>>>
    {
        private readonly IDbContext _dbcontext;
        private readonly IMapper _mapper;

        public GetStylesRequestHandler(ICacheProvider cacheProvider, IDbContext dbcontext, IMapper mapper)
        {
            _dbcontext = dbcontext ?? throw new ArgumentNullException(nameof(dbcontext));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        public override HandlerResult<IEnumerable<ThemeDto>> HandleRequest(GetStylesRequest request, CancellationToken cancellationToken, HandlerResult<IEnumerable<ThemeDto>> result)
        {
            var allUsers = _dbcontext.AppUsers.ToList();
            var styles = _dbcontext.Themes.Where(x => !x.Deleted);

            result.Entity = _mapper.Map<List<ThemeDto>>(styles);
            foreach (var style in result.Entity)
            {
                var user = allUsers.FirstOrDefault(u => u.Id == style.CreatedById);
                if (user != null)
                {
                    style.CreatedByName = user.UserName;
                }
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