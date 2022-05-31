using DAM.Application.Projects.Dtos;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace DAM.Application.Projects.Requests
{
    public class RemoveAssetsFromProjectRequest : IRequest<HandlerResult<ImportAssetsToProjectDto>>
    {
        public string UserId { get; set; }
        public ImportAssetsToProjectDto AssetsToRemove { get; set; }
        public RemoveAssetsFromProjectRequest(ImportAssetsToProjectDto assetsToRemove, string userId)
        {
            AssetsToRemove = assetsToRemove;
            UserId = userId;
        }
    }
}
