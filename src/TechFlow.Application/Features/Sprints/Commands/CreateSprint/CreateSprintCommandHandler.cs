
using MediatR;
using TechFlow.Application.Common.Errors;
using TechFlow.Application.Common.Interfaces;
using TechFlow.Application.Common.Interfaces.Repositories;
using TechFlow.Application.Common.Services;
using TechFlow.Application.Features.Sprints.Commands.CreateSprint;
using TechFlow.Application.Features.Sprints.Dtos;
using TechFlow.Domain.Common.Results;
using TechFlow.Domain.Projects;
using TechFlow.Domain.Roles;
using TechFlow.Domain.Sprints;

public sealed class CreateSprintCommandHandler(
    IUnitOfWork unitOfWork,
    IUser currentUser,
    ProjectAccessService accessService)
    : IRequestHandler<CreateSprintCommand, Result<SprintSummaryDto>>
{
    public async Task<Result<SprintSummaryDto>> Handle(
        CreateSprintCommand command,
        CancellationToken ct)
    {
        if (currentUser.Id is null)
            return ApplicationErrors.Unauthorized;

        var project = await unitOfWork.Projects.GetByIdWithMembersAsync(command.ProjectId, ct);
        if (project is null)
            return ProjectErrors.NotFound;

        var isAdmin = currentUser.IsInRole(SystemRoles.Admin);
        if (!accessService.CanModify(project, currentUser.Id.Value, isAdmin))
            return ProjectErrors.AccessDenied;

        var nextNumber = await unitOfWork.Sprints.GetNextSprintNumberAsync(command.ProjectId, ct);

        var result = Sprint.Create(
            projectId: command.ProjectId,
            companyId: project.CompanyId,
            createdByUserId: currentUser.Id.Value,
            sprintNumber: nextNumber,
            startDate: command.StartDate,
            endDate: command.EndDate,
            name: command.Name,
            goal: command.Goal);

        if (result.IsFailure)
            return result.TopError;

        var sprint = result.Value;

        unitOfWork.Sprints.Add(sprint);
        await unitOfWork.SaveChangesAsync(ct);

        return new SprintSummaryDto(
            Id: sprint.Id,
            ProjectId: sprint.ProjectId,
            SprintNumber: sprint.SprintNumber,
            DisplayName: sprint.DisplayName,
            Name: sprint.Name,
            Goal: sprint.Goal,
            Status: sprint.Status,
            StartDate: sprint.StartDate,
            EndDate: sprint.EndDate,
            ActualEndDate: sprint.ActualEndDate,
            IsLocked: sprint.IsLocked,
            TotalTasks: 0,
            CompletedTasks: 0);
    }
}
