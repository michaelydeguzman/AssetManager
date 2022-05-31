using DAM.Application.Assets.Dtos;
using MediatR;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Text;

namespace DAM.Application.Assets.Requests
{
    public class DownloadOfficeToPDFRequest : IRequest<HandlerResult<HttpResponseMessage>>
    {
        public string EmailAddress { get; set; }
        public string AssetKey { get; set; }

        public DownloadOfficeToPDFRequest(string assetKey, string emailAddress)
        {
            AssetKey = assetKey;
            EmailAddress = emailAddress;
        }
    }
}
