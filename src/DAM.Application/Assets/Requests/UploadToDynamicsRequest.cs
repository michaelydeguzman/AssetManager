using System;
using System.Collections.Generic;
using MediatR;
using DAM.Application.Assets.Dtos;

namespace DAM.Application.Assets.Requests
{
    public class UploadToDynamicsRequest : IRequest<HandlerResult<List<UploadToDynamicsDto>>>
    {
        public List<UploadToDynamicsDto> UploadToDynamics { get; private set; }

        public UploadToDynamicsRequest(List<UploadToDynamicsDto> uploadToDynamics)
        {
            UploadToDynamics = uploadToDynamics;
        }
    }
}
