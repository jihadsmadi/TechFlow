using MediatR;
using TechFlow.Domain.Common.Results;

namespace TechFlow.Application.Features.Projects.Commands.ArchiveProject;

public sealed record ArchiveProjectCommand(Guid Id) : IRequest<Result<Updated>>;
