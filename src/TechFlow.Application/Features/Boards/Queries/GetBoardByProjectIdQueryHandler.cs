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

namespace TechFlow.Application.Features.Boards.Queries.GetBoardByProjectId;

public sealed class GetBoardByProjectIdQueryHandler(
    IUnitOfWork unitOfWork,
    IUser currentUser,
    ProjectAccessService accessService)
    : IRequestHandler<GetBoardByProjectIdQuery, Result<BoardDto>>
{
    public async Task<Result<BoardDto>> Handle(
        GetBoardByProjectIdQuery query,
        CancellationToken ct)
    {
        if (currentUser.Id is null)
            return ApplicationErrors.Unauthorized;

        var project = await unitOfWork.Projects.GetByIdWithMembersAsync(query.ProjectId, ct);
        if (project is null)
            return ProjectErrors.NotFound;

        var isAdmin = currentUser.IsInRole(SystemRoles.Admin);

        if (!accessService.CanAccess(project, currentUser.Id.Value, isAdmin))
            return ProjectErrors.AccessDenied;

       
        var board = await unitOfWork.Boards.GetByProjectIdWithListsAsync(query.ProjectId, ct);
        if (board is null)
            return BoardErrors.NotFound;

        return new BoardDto(
            Id:        board.Id,
            ProjectId: board.ProjectId,
            Name:      board.Name,
            Lists:     board.Lists
                .OrderBy(l => l.DisplayOrder)
                .Select(l => new ListDto(
                    Id:              l.Id,
                    BoardId:         l.BoardId,
                    Name:            l.Name,
                    Color:           l.Color,
                    DisplayOrder:    l.DisplayOrder,
                    IsDefault:       l.IsDefault,
                    IsCompletedList: l.IsCompletedList,
                    CreatedAt:       l.CreatedAtUtc,
                    UpdatedAt:       l.LastModifiedUtc))
                .ToList(),
            CreatedAt: board.CreatedAtUtc ,
            UpdatedAt: board.LastModifiedUtc);
    }
}
