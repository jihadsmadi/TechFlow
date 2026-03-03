using MediatR;
using TechFlow.Domain.Common.Results;
namespace TechFlow.Application.Features.Services.ClearingCache.Commands;

public record ClearCacheCommand(string? CacheKey = null) : IRequest<Result<bool>>;
