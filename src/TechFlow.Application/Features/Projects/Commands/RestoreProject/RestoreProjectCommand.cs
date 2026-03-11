using MediatR;
using TechFlow.Domain.Common.Results;

namespace TechFlow.Application.Features.Projects.Commands.RestoreProject;

public sealed record RestoreProjectCommand(Guid Id) : IRequest<Result<Updated>>;
