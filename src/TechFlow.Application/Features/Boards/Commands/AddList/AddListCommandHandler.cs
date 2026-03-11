using MediatR;
using TechFlow.Application.Common.Errors;
using TechFlow.Application.Common.Interfaces;
using TechFlow.Application.Common.Interfaces.Repositories;
using TechFlow.Application.Common.Services;
using TechFlow.Application.Features.Boards.DTOs;
using TechFlow.Domain.Boards;
using TechFlow.Domain.Common.Results;
using TechFlow.Domain.Projects;
using TechFlow.Domain.Roles;


namespace TechFlow.Application.Features.Boards.Commands.AddList;

public sealed class AddListCommandHandler(
    IUnitOfWork unitOfWork,
    IUser currentUser,
    ProjectAccessService accessService)
    : IRequestHandler<AddListCommand, Result<ListSummaryDto>>
{
    public async Task<Result<ListSummaryDto>> Handle(AddListCommand command, CancellationToken ct)
    {
        if (currentUser.Id is null)
            return ApplicationErrors.Unauthorized;

        var project = await unitOfWork.Projects.GetByIdWithMembersAsync(command.ProjectId, ct);
        if (project is null)
            return ProjectErrors.NotFound;

        var isAdmin = currentUser.IsInRole(SystemRoles.Admin);
        if (!accessService.CanModify(project, currentUser.Id.Value, isAdmin))
            return ProjectErrors.AccessDenied;

        var board = await unitOfWork.Boards.GetByProjectIdWithListsAsync(command.ProjectId, ct);
        if (board is null)
            return BoardErrors.NotFound;

        var result = board.AddList(command.Name, command.Color);
        if (result.IsFailure)
            return result.TopError;

        unitOfWork.Boards.MarkListAsAdded(result.Value);

        await unitOfWork.SaveChangesAsync(ct);

        var list = result.Value;
        return new ListSummaryDto(
            Id:              list.Id,
            Name:            list.Name,
            Color:           list.Color,
            DisplayOrder:    list.DisplayOrder,
            IsDefault:       list.IsDefault,
            IsCompletedList: list.IsCompletedList);
    }
}
