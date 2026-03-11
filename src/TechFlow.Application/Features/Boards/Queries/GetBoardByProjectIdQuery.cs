using MediatR;
using TechFlow.Application.Features.Boards.DTOs;
using TechFlow.Domain.Common.Results;

namespace TechFlow.Application.Features.Boards.Queries.GetBoardByProjectId;

public sealed record GetBoardByProjectIdQuery(Guid ProjectId)
    : IRequest<Result<BoardDto>>;
