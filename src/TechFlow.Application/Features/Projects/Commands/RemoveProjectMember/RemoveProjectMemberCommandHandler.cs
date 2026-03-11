using MediatR;
using TechFlow.Application.Common.Errors;
using TechFlow.Application.Common.Interfaces;
using TechFlow.Application.Common.Interfaces.Repositories;
using TechFlow.Application.Common.Services;
using TechFlow.Domain.Common.Results;
using TechFlow.Domain.Projects;
using TechFlow.Domain.Roles;


namespace TechFlow.Application.Features.Projects.Commands.RemoveProjectMember;

public sealed class RemoveProjectMemberCommandHandler(
    IUnitOfWork unitOfWork,
    IUser currentUser,
    ProjectAccessService accessService)
    : IRequestHandler<RemoveProjectMemberCommand, Result<Deleted>>
{
    public async Task<Result<Deleted>> Handle(RemoveProjectMemberCommand command, CancellationToken ct)
    {
        if (currentUser.Id is null)
            return ApplicationErrors.Unauthorized;

        var project = await unitOfWork.Projects.GetByIdWithMembersAsync(command.ProjectId, ct);
        if (project is null)
            return ProjectErrors.NotFound;

        var isAdmin = currentUser.IsInRole(SystemRoles.Admin);

        if (!accessService.CanManageMembers(project, currentUser.Id.Value, isAdmin))
            return ProjectErrors.AccessDenied;

        var result = project.RemoveMember(command.UserId);
        if (result.IsFailure)
            return result.TopError;

        await unitOfWork.SaveChangesAsync(ct);
        return Result.Deleted;
    }
}
