using MediatR;
using TechFlow.Application.Common.Errors;
using TechFlow.Application.Common.Interfaces;
using TechFlow.Application.Common.Interfaces.Repositories;
using TechFlow.Application.Common.Services;
using TechFlow.Domain.Common.Results;
using TechFlow.Domain.Projects;
using TechFlow.Domain.Roles;
using TechFlow.Domain.Users;

namespace TechFlow.Application.Features.Projects.Commands.AddProjectMember;

public sealed class AddProjectMemberCommandHandler(
    IUnitOfWork unitOfWork,
    IUser currentUser,
    ProjectAccessService accessService)
    : IRequestHandler<AddProjectMemberCommand, Result<Created>>
{
    public async Task<Result<Created>> Handle(AddProjectMemberCommand command, CancellationToken ct)
    {
        if (currentUser.Id is null)
            return ApplicationErrors.Unauthorized;

        var project = await unitOfWork.Projects.GetByIdWithMembersAsync(command.ProjectId, ct);
        if (project is null)
            return ProjectErrors.NotFound;

        var isAdmin = currentUser.IsInRole(SystemRoles.Admin);

        if (!accessService.CanManageMembers(project, currentUser.Id.Value, isAdmin))
            return ProjectErrors.AccessDenied;

        // verify the user exists in the same company
        var user = await unitOfWork.Users.GetByIdAsync(command.UserId, ct);
        if (user is null)
            return UserErrors.NotFound;

        if (user.CompanyId != project.CompanyId)
            return ProjectErrors.UserNotInCompany;

        var result = project.AddMember(command.UserId, addedByUserId: currentUser.Id.Value);
        if (result.IsFailure)
            return result.TopError;

        await unitOfWork.SaveChangesAsync(ct);
        return Result.Created;
    }
}
