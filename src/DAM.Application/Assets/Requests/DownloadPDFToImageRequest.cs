using DAM.Application.Assets.Dtos;
using MediatR;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace DAM.Application.Assets.Requests
{
    public class DownloadPDFToImageRequest : IRequest<HandlerResult<Dictionary<string, Stream>>>
    {
        public ShareDto ShareDto { get; private set; }
        public DownloadPDFToImageRequest(string assetKey, string emailAddress, bool showWaterMark, string extension = "")
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
