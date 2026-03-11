using MediatR;
using TechFlow.Application.Common.Errors;
using TechFlow.Application.Common.Interfaces;
using TechFlow.Application.Common.Interfaces.Repositories;
using TechFlow.Application.Common.Services;
using TechFlow.Domain.Common.Results;
using TechFlow.Domain.Projects;
using TechFlow.Domain.Roles;

namespace TechFlow.Application.Features.Projects.Commands.UpdateProject;

public sealed class UpdateProjectCommandHandler(
    IUnitOfWork unitOfWork,
    IUser currentUser,
    ProjectAccessService accessService)
    : IRequestHandler<UpdateProjectCommand, Result<Updated>>
{
    public async Task<Result<Updated>> Handle(
        UpdateProjectCommand command,
        CancellationToken ct)
    {
        if (currentUser.Id is null)
            return ApplicationErrors.Unauthorized;

        var project = await unitOfWork.Projects.GetByIdWithMembersAsync(command.Id, ct);
        if (project is null)
            return ProjectErrors.NotFound;

        var isAdmin = currentUser.IsInRole(SystemRoles.Admin);

        if (!accessService.CanModify(project, currentUser.Id.Value, isAdmin))
            return ProjectErrors.AccessDenied;

        // check for name conflict (exclude self)
        if (!string.Equals(project.Name, command.Name, StringComparison.OrdinalIgnoreCase))
        {
            var nameExists = await unitOfWork.Projects.NameExistsInCompanyAsync(
                project.CompanyId, command.Name, excludeId: command.Id, ct: ct);

            if (nameExists)
                return ProjectErrors.NameAlreadyExists;
        }

        var result = project.Update(
            command.Name,
            command.Description,
            command.Color,
            command.StartDate,
            command.EndDate);

        if (result.IsFailure)
            return result.TopError;

        await unitOfWork.SaveChangesAsync(ct);
        return Result.Updated;
    }
}
