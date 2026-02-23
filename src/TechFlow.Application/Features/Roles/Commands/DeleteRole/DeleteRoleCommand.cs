using MediatR;
using TechFlow.Domain.Common.Results;

namespace TechFlow.Application.Features.Roles.Commands.DeleteRole;

public sealed record DeleteRoleCommand(Guid Id) : IRequest<Result<Deleted>>;