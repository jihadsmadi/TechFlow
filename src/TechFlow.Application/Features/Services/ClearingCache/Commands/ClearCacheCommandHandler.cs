using MediatR;
using Microsoft.Extensions.Caching.Hybrid;
using TechFlow.Application.Common.Constants;
using TechFlow.Application.Features.Services.ClearingCache.Commands;
using TechFlow.Domain.Common.Results;

namespace TechFlow.Application.Features.Services.ClearingCache.Commands;

public class ClearCacheCommandHandler(HybridCache _cache) : IRequestHandler<ClearCacheCommand, Result<bool>>
{
    public async Task<Result<bool>> Handle(ClearCacheCommand request, CancellationToken cancellationToken)
    {
        try
        {
            if (request.CacheKey is not null)
                await _cache.RemoveAsync(request.CacheKey);
            else
            {
                foreach (var cacheKey in CacheKeys.AllTags)
                    await _cache.RemoveByTagAsync(cacheKey);
            }
        }
        catch (Exception)
        {
            return false;
        }
        
        return true;
    }
}