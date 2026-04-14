using MediatR;
using TechFlow.Application.Common.Errors;
using TechFlow.Application.Common.Interfaces;
using TechFlow.Application.Common.Interfaces.Repositories;
using TechFlow.Application.Common.Services;
using TechFlow.Application.Features.Tasks.Dtos;
using TechFlow.Application.Features.Tasks.Mappers;
using TechFlow.Domain.Common.Results;
using TechFlow.Domain.Projects;
using TechFlow.Domain.Roles;
using TechFlow.Domain.Tasks;

namespace TechFlow.Application.Features.Tasks.Commands.CreateTask;

public sealed class CreateTaskCommandHandler(
    IUnitOfWork unitOfWork,
    IUser currentUser,
    ProjectAccessService accessService)
    : IRequestHandler<CreateTaskCommand, Result<TaskSummaryDto>>
{
    public async Task<Result<TaskSummaryDto>> Handle(CreateTaskCommand command, CancellationToken ct)
    {
        if (currentUser.Id is null)
            return ApplicationErrors.Unauthorized;

        var project = await unitOfWork.Projects.GetByIdWithMembersAsync(command.ProjectId, ct);
        if (project is null)
            return ProjectErrors.NotFound;

        var isAdmin = currentUser.IsInRole(SystemRoles.Admin);
        if (!accessService.CanAccess(project, currentUser.Id.Value, isAdmin))
            return ProjectErrors.AccessDenied;

        var list = await unitOfWork.Boards.GetListByIdAsync(command.ListId, ct);
        if (list is null)
            return TaskErrors.ListNotFound;

        var isListInProject = await unitOfWork.Boards.ListBelongsToProjectAsync(command.ListId, command.ProjectId, ct);
        if (!isListInProject)
            return TaskErrors.ListNotBelongToProject;

        var maxOrder = await unitOfWork.Tasks.GetMaxDisplayOrderInListAsync(command.ListId, ct);
        var displayOrder = maxOrder + 1.0;

        var priority = command.Priority ?? project.Settings.DefaultPriority;
        var type = command.Type ?? project.Settings.DefaultTaskType;

        if (project.Settings.RequireEstimate && !command.EstimatedMinutes.HasValue)
            return TaskErrors.EstimateRequired;

        var createResult = Domain.Tasks.Task.Create(
            listId: command.ListId,
            companyId: project.CompanyId,
            projectId: command.ProjectId,
            createdByUserId: currentUser.Id.Value,
            title: command.Title,
            priority: priority,
            type: type,
            displayOrder: displayOrder,
            description: command.Description,
            dueDate: command.DueDate,
            estimatedMinutes: command.EstimatedMinutes);

        if (createResult.IsFailure)
            return createResult.TopError;

        var task = createResult.Value;

        if (project.Settings.AutoAssignCreator)
        {
            var assignResult = task.AssignUser(currentUser.Id.Value, currentUser.Id.Value);
            if (assignResult.IsFailure)
                return assignResult.TopError;
        }

        unitOfWork.Tasks.Add(task);
        await unitOfWork.SaveChangesAsync(ct);

        var taskWithDetails = await unitOfWork.Tasks.GetByIdWithAssignmentsAsync(task.Id, ct);

        // 11. Return DTO
        return taskWithDetails is not null ? taskWithDetails.ToSummaryDto() : task.ToSummaryDto();
    }
}