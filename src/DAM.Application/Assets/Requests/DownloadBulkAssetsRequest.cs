using DAM.Application.Assets.Dtos;
using MediatR;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace DAM.Application.Assets.Requests
{
    public class DownloadBulkAssetsRequest : IRequest<HandlerResult<Dictionary<string, Stream>>>
    {
        public string AssetsIds { get; private set; }
        public string EmailAddress { get; private set; }
        public bool ShowWaterMark { get; private set; }
        public DownloadBulkAssetsRequest(string assetsIds, string emailAddress, bool showWaterMark)
        {
            AssetsIds = assetsIds;
            EmailAddress = emailAddress;
            ShowWaterMark = showWaterMark;
        }
    }
}
