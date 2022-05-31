using DAM.Application.Assets.Dtos;
using MediatR;

namespace DAM.Application.Assets.Requests
{
    public class ConvertPDFToThumbnailImageRequest : IRequest<HandlerResult<DownloadDto>>
    {

        public AssetThumbnailDto AssetThumbnailDto { get; private set; }

        public string UserId { get; private set; }

        public ConvertPDFToThumbnailImageRequest(AssetThumbnailDto asset, string userId)
        {
            AssetThumbnailDto = asset;
            UserId = userId;
        }
    }
}
