using MediatR;

namespace DAM.Application.Assets.Requests
{
    public class UpdateAssetsUrlRequest : IRequest<HandlerResult<string>>
    {
        public UpdateAssetsUrlRequest() {}
    }
}