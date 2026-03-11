using MediatR;
using TechFlow.Application.Features.Boards.DTOs;
using TechFlow.Domain.Common.Results;


namespace TechFlow.Application.Features.Boards.Commands.AddList;

public sealed record AddListCommand(
    Guid ProjectId,
    string Name,
    string? Color = null) : IRequest<Result<ListSummaryDto>>;
