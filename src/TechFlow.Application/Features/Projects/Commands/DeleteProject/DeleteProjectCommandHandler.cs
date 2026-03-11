using MediatR;
using TechFlow.Application.Common.Errors;
using TechFlow.Application.Common.Interfaces;
using TechFlow.Application.Common.Interfaces.Repositories;
using TechFlow.Domain.Common.Results;
using TechFlow.Domain.Projects;
using TechFlow.Domain.Roles;


namespace TechFlow.Application.Features.Projects.Commands.DeleteProject;

public sealed class DeleteProjectCommandHandler(
    IUnitOfWork unitOfWork,
    IUser currentUser)
    : IRequestHandler<DeleteProjectCommand, Result<Deleted>>
{
    public async Task<Result<Deleted>> Handle(DeleteProjectCommand command, CancellationToken ct)
    {
        if (currentUser.Id is null)
            return ApplicationErrors.Unauthorized;

        var project = await unitOfWork.Projects.GetByIdAsync(command.Id, ct);
        if (project is null)
            return ProjectErrors.NotFound;

        // only admin can delete — deletion is destructive
        var isAdmin = currentUser.IsInRole(SystemRoles.Admin);
        if (!isAdmin)
            return ProjectErrors.AccessDenied;

        unitOfWork.Projects.Remove(project);
        await unitOfWork.SaveChangesAsync(ct);
        return Result.Deleted;
    }
}
