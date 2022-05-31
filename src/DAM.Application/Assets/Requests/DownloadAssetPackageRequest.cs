using DAM.Application.Assets.Dtos;
using MediatR;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace DAM.Application.Assets.Requests
{
    public class DownloadAssetPackageRequest : IRequest<HandlerResult<DownloadDto>>
    {
        public ShareDto ShareDto { get; private set; }
        public string UserId { get; private set; }
        public DownloadAssetPackageRequest(string assetKey, string userId, bool showWaterMark, string extension = "")
        {
            ShareDto = new ShareDto() 
            { 
                AssetKey = assetKey,
                ShowWaterMark = showWaterMark,
                Extension = extension
            };
            UserId = userId;
        }
    }
}
