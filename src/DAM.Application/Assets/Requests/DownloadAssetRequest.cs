using DAM.Application.Assets.Dtos;
using MediatR;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace DAM.Application.Assets.Requests
{
    public class DownloadAssetRequest : IRequest<HandlerResult<DownloadDto>>
    {
        public ShareDto ShareDto { get; private set; }
        public DownloadAssetRequest(string assetKey, string emailAddress, bool showWaterMark, string extension = "")
        {
            ShareDto = new ShareDto() 
            { 
                AssetKey = assetKey,
                EmailAddress = emailAddress,
                ShowWaterMark = showWaterMark,
                Extension = extension
            };
        }
    }
}
