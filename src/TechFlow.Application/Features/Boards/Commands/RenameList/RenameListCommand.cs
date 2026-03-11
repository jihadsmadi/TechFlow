using MediatR;
using TechFlow.Domain.Common.Results;


namespace TechFlow.Application.Features.Boards.Commands.RenameList;

public sealed record RenameListCommand(
    Guid ProjectId,
    Guid ListId,
    string Name) : IRequest<Result<Updated>>;
