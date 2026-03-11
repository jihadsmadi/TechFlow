using MediatR;
using TechFlow.Domain.Common.Results;


namespace TechFlow.Application.Features.Boards.Commands.SetListColor;

public sealed record SetListColorCommand(
    Guid ProjectId,
    Guid ListId,
    string? Color) : IRequest<Result<Updated>>;
