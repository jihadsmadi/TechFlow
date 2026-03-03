using MediatR;
using Microsoft.Extensions.Caching.Hybrid;
using TechFlow.Application.Common.Interfaces;
using TechFlow.Domain.Common.Results.Abstractions;

namespace TechFlow.Application.Common.Behaviours;

public sealed class CacheInvalidationBehavior<TRequest, TResponse>(
    HybridCache cache)
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken ct)
    {
        var response = await next();

        if (request is ICacheInvalidator invalidator)
        {
            var isSuccess = response is IResult r && r.IsSuccess;

            if (isSuccess)
            {
                foreach (var tag in invalidator.Tags)
                    await cache.RemoveByTagAsync(tag, ct);
            }
        }

        return response;
    }
}
