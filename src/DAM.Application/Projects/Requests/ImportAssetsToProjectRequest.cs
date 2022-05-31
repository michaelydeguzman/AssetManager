using DAM.Application.Projects.Dtos;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace DAM.Application.Projects.Requests
{
    public class ImportAssetsToProjectRequest : IRequest<HandlerResult<ImportAssetsToProjectDto>>
    {
        public string UserId { get; set; }
        public ImportAssetsToProjectDto Imports { get; set; }
        public ImportAssetsToProjectRequest(ImportAssetsToProjectDto imports, string userId)
        {
            Imports = imports;
            UserId = userId;
        }
    }
}
