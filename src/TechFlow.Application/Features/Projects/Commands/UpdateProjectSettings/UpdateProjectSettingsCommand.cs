using MediatR;
using TechFlow.Domain.Common.Results;

namespace TechFlow.Application.Features.Projects.Commands.UpdateProjectSettings;

public sealed record UpdateProjectSettingsCommand(
    Guid ProjectId,
    List<string> DefaultListNames,
    string DefaultTaskType,
    string DefaultPriority,
    bool AutoAssignCreator,
    bool RequireEstimate,
    bool AllowSubtasks) : IRequest<Result<Updated>>;
