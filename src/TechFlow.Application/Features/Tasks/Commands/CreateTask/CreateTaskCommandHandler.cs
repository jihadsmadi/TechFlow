using MediatR;
using TechFlow.Application.Common.Errors;
using TechFlow.Application.Common.Interfaces;
using TechFlow.Application.Common.Interfaces.Repositories;
using TechFlow.Application.Common.Services;
using TechFlow.Application.Features.Tasks.DTOs;
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

        if (project.Settings.RequireEstimate && command.EstimatedMinutes is null)
            return TaskErrors.EstimateRequired;


        var maxOrder = await unitOfWork.Tasks.GetMaxDisplayOrderInListAsync(command.ListId, ct);

        var result = Domain.Tasks.Task.Create(
            listId: command.ListId,
            companyId: project.CompanyId,
            projectId: command.ProjectId,
            createdByUserId: currentUser.Id.Value,
            title: command.Title,
            priority: command.Priority ?? project.Settings.DefaultPriority,
            type: command.Type ?? project.Settings.DefaultTaskType,
            displayOrder: maxOrder + 1.0,
            description: command.Description,
            dueDate: command.DueDate,
            estimatedMinutes: command.EstimatedMinutes);

        if (result.IsFailure)
            return result.TopError;

        var task = result.Value;

        if (project.Settings.AutoAssignCreator)
            task.AssignUser(currentUser.Id.Value, currentUser.Id.Value);

        unitOfWork.Tasks.Add(task);
        await unitOfWork.SaveChangesAsync(ct);

        return new TaskSummaryDto(
            Id: task.Id,
            ListId: task.ListId,
            Title: task.Title,
            Priority: task.Priority,
            Type: task.Type,
            DisplayOrder: task.DisplayOrder,
            DueDate: task.DueDate,
            IsCompleted: task.IsCompleted,
            SubtasksTotal: 0,
            SubtasksCompleted: 0);
    }
}