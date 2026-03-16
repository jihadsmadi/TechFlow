using MediatR;
using TechFlow.Application.Features.Tasks.DTOs;
using TechFlow.Domain.Common.Results;


namespace TechFlow.Application.Features.Tasks.Commands.AddSubtask;

public sealed record AddSubtaskCommand(
    Guid ProjectId,
    Guid TaskId,
    string Title) : IRequest<Result<SubtaskDto>>;
