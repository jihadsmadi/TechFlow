using MediatR;
using TechFlow.Application.Common.Errors;
using TechFlow.Application.Common.Interfaces;
using TechFlow.Application.Common.Interfaces.Repositories;
using TechFlow.Application.Common.Services;
using TechFlow.Application.Features.Projects.Dtos;
using TechFlow.Domain.Common.Results;
using TechFlow.Domain.Projects;
using TechFlow.Domain.Roles;

namespace TechFlow.Application.Features.Projects.Queries.GetProjectById;

public sealed class GetProjectByIdQueryHandler(
    IUnitOfWork unitOfWork,
    IUser currentUser,
    ProjectAccessService accessService)
    : IRequestHandler<GetProjectByIdQuery, Result<ProjectDto>>
{
    public async Task<Result<ProjectDto>> Handle(
        GetProjectByIdQuery query,
        CancellationToken ct)
    {
        if (currentUser.Id is null)
            return ApplicationErrors.Unauthorized;

        var project = await unitOfWork.Projects.GetByIdWithMembersAsync(query.Id, ct);
        if (project is null)
            return ProjectErrors.NotFound;

        var isAdmin = currentUser.IsInRole(SystemRoles.Admin);

        if (!accessService.CanAccess(project, currentUser.Id.Value, isAdmin))
            return ProjectErrors.AccessDenied;

        return new ProjectDto(
            Id:              project.Id,
            CompanyId:       project.CompanyId,
            CreatedByUserId: project.CreatedByUserId,
            Name:            project.Name,
            Description:     project.Description,
            Status:          project.Status,
            Color:           project.Color.Value,
            StartDate:       project.StartDate,
            EndDate:         project.EndDate,
            IsArchived:      project.IsArchived,
            ArchivedAt:      project.ArchivedAt,
            MemberCount:     project.Members.Count,
            Settings: new ProjectSettingsDto(
                DefaultListNames: project.Settings.GetDefaultListNames().ToList(),
                DefaultTaskType:  project.Settings.DefaultTaskType,
                DefaultPriority:  project.Settings.DefaultPriority,
                AutoAssignCreator: project.Settings.AutoAssignCreator,
                RequireEstimate:  project.Settings.RequireEstimate,
                AllowSubtasks:    project.Settings.AllowSubtasks),
            CreatedAt: project.CreatedAtUtc,
            UpdatedAt: project.LastModifiedUtc);
    }
}
