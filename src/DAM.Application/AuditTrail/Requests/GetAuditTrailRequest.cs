using DAM.Application.AuditTrail.Dtos;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace DAM.Application.AuditTrail.Requests
{
    public class GetAuditTrailRequest : IRequest<HandlerResult<AuditTrailDto>>
    {
        public FetchAuditDto FetchAuditParams { get; set; }
        
        public GetAuditTrailRequest(FetchAuditDto fetchAuditParams)
        {
            FetchAuditParams = fetchAuditParams;
        }
    }
}
