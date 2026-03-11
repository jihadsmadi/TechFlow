using MediatR;
using TechFlow.Application.Common.Errors;
using TechFlow.Application.Common.Interfaces;
using TechFlow.Application.Common.Interfaces.Repositories;
using TechFlow.Application.Features.Projects.Dtos;
using TechFlow.Domain.Common.Results;
using TechFlow.Domain.Projects;

namespace TechFlow.Application.Features.Projects.Commands.CreateProject;

// ── Handler 

public sealed class CreateProjectCommandHandler(
    IUnitOfWork unitOfWork,
    IUser currentUser)
    : IRequestHandler<CreateProjectCommand, Result<ProjectSummaryDto>>
{
    public async Task<Result<ProjectSummaryDto>> Handle(
        CreateProjectCommand command,
        CancellationToken ct)
    {
        if (currentUser.Id is null)
            return ApplicationErrors.Unauthorized;


        var userId    = currentUser.Id.Value;
        var companyId = currentUser.CompanyId;

        var nameExists = await unitOfWork.Projects.NameExistsInCompanyAsync(
            companyId, command.Name, ct: ct);

        if (nameExists)
            return ProjectErrors.NameAlreadyExists;

        var result = Project.Create(
            companyId:       companyId,
            createdByUserId: userId,
            name:            command.Name,
            description:     command.Description,
            color:           command.Color,
            startDate:       command.StartDate,
            endDate:         command.EndDate);

        if (result.IsFailure)
            return result.TopError;

        var project = result.Value;

        unitOfWork.Projects.Add(project);
        await unitOfWork.SaveChangesAsync(ct);

        return new ProjectSummaryDto(
            Id:          project.Id,
            Name:        project.Name,
            Description: project.Description,
            Status:      project.Status,
            Color:       project.Color.Value,
            IsArchived:  project.IsArchived,
            MemberCount: project.Members.Count,
            StartDate:   project.StartDate,
            EndDate:     project.EndDate);
    }
}
