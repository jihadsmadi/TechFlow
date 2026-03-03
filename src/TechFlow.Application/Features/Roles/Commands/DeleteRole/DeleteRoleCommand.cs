using MediatR;
using TechFlow.Application.Common.Constants;
using TechFlow.Application.Common.Interfaces;
using TechFlow.Domain.Common.Results;

namespace TechFlow.Application.Features.Roles.Commands.DeleteRole;

public sealed record DeleteRoleCommand(Guid Id) : IRequest<Result<Deleted>>, ICacheInvalidator
{
    public string[] Tags => [CacheKeys.Roles.Tag];
}