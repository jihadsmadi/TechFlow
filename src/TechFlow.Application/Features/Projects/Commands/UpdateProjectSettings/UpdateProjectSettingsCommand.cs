using MediatR;
using TechFlow.Domain.Common.Results;

namespace TechFlow.Application.Features.Projects.Commands.UpdateProjectSettings;

public sealed record UpdateProjectSettingsCommand(
    Guid ProjectId,
    List<string>? DefaultListNames = null,
    string? DefaultTaskType = null,
    string? DefaultPriority = null,
    bool? AutoAssignCreator = null,
    bool? RequireEstimate = null,
    bool? AllowSubtasks = null,
    bool? SprintLockOnStart = null,
    int? SprintDurationDays = null,
    string? IncompleteTasksAction = null
) : IRequest<Result<Updated>>;

