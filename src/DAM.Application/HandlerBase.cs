using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace DAM.Application
{
    public abstract class HandlerBase<TRequest, TResult> : IRequestHandler<TRequest, TResult> where TRequest : IRequest<TResult> where TResult : class, new()
    {
        public abstract TResult HandleRequest(TRequest request, CancellationToken cancellationToken, TResult result);

        public virtual Exception OnException(Exception ex)
        {
            return ex;
        }

        public async Task<TResult> Handle(TRequest request, CancellationToken cancellationToken)
        {
            return await Task.Run(() => Execute(request, (result) => HandleRequest(request, cancellationToken, result)));
        }

        private TResult Execute(TRequest request, Action<TResult> action)
        {
            var result = new TResult();

            if (result != null)
            {
                try
                {
                    action(result);
                }
                catch (Exception ex)
                {
                    //TODO: Log exception
                    var managedException = OnException(ex);

                    throw managedException;
                }
            }

            return result;
        }
    }
}