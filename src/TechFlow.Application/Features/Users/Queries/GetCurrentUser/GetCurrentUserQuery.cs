using MediatR;
using TechFlow.Application.Common.Constants;
using TechFlow.Application.Common.Interfaces;
using TechFlow.Application.Features.Users.Dtos;
using TechFlow.Domain.Common.Results;

namespace TechFlow.Application.Features.Users.Queries.GetCurrentUser;

public sealed record GetCurrentUserQuery(Guid UserId)
    : IRequest<Result<UserProfileDto>>, ICachedQuery
{
    public string CacheKey => CacheKeys.Users.ById(UserId);
    public TimeSpan Expiration => TimeSpan.FromMinutes(CacheKeys.Users.ExpirationMinutes);
    public string[] Tags => [CacheKeys.Users.Tag];
}