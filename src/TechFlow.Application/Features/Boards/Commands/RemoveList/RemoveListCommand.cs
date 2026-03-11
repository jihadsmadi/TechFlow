using MediatR;
using TechFlow.Domain.Common.Results;


namespace TechFlow.Application.Features.Boards.Commands.RemoveList;

public sealed record RemoveListCommand(
    Guid ProjectId,
    Guid ListId) : IRequest<Result<Deleted>>;
