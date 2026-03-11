using MediatR;
using TechFlow.Application.Common.Errors;
using TechFlow.Application.Common.Interfaces;
using TechFlow.Application.Common.Interfaces.Repositories;
using TechFlow.Domain.Common.Results;
using TechFlow.Domain.Projects;
using TechFlow.Domain.Roles;

namespace TechFlow.Application.Features.Projects.Commands.RestoreProject;

public sealed class RestoreProjectCommandHandler(
    IUnitOfWork unitOfWork,
    IUser currentUser)
    : IRequestHandler<RestoreProjectCommand, Result<Updated>>
{
    public async Task<Result<Updated>> Handle(RestoreProjectCommand command, CancellationToken ct)
    {
        if (currentUser.Id is null)
            return ApplicationErrors.Unauthorized;

        var project = await unitOfWork.Projects.GetByIdWithMembersAsync(command.Id, ct);
        if (project is null)
            return ProjectErrors.NotFound;

        var isAdmin = currentUser.IsInRole(SystemRoles.Admin);

        // only admin or creator can restore
        if (!isAdmin && project.CreatedByUserId != currentUser.Id.Value)
            return ProjectErrors.AccessDenied;

        var result = project.Restore();
        if (result.IsFailure)
            return result.TopError;

        await unitOfWork.SaveChangesAsync(ct);
        return Result.Updated;
    }
}
