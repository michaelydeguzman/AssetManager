using DAM.Application.FeatureFlags.Dtos;
using DAM.Application.UserRoles.Dtos;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace DAM.Application.FeatureFlags.Requests
{
    public class GetFeatureFlagsRequest : IRequest<HandlerResult<IEnumerable<FeatureFlagDto>>>
    {
        public GetFeatureFlagsRequest()
        {

        }
    }
}