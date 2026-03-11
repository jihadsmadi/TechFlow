using MediatR;
using TechFlow.Domain.Common.Results;


namespace TechFlow.Application.Features.Boards.Commands.RenameBoard;

public sealed record RenameBoardCommand(
    Guid ProjectId,
    string Name) : IRequest<Result<Updated>>;
