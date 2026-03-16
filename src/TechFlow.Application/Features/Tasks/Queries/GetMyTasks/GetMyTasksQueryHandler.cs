using MediatR;
using TechFlow.Application.Common.Errors;
using TechFlow.Application.Common.Interfaces;
using TechFlow.Application.Common.Interfaces.Repositories;
using TechFlow.Application.Features.Tasks.DTOs;
using TechFlow.Domain.Common.Results;

namespace TechFlow.Application.Features.Tasks.Queries.GetMyTasks;

public sealed class GetMyTasksQueryHandler(
    IUnitOfWork unitOfWork,
    IUser currentUser)
    : IRequestHandler<GetMyTasksQuery, Result<IReadOnlyList<MyTaskDto>>>
{
    public async Task<Result<IReadOnlyList<MyTaskDto>>> Handle(
        GetMyTasksQuery query,
        CancellationToken ct)
    {
        if (currentUser.Id is null)
            return ApplicationErrors.Unauthorized;

        var tasks = await unitOfWork.Tasks.GetByAssigneeAsync(
            userId: currentUser.Id.Value,
            companyId: currentUser.CompanyId,
            includeCompleted: query.IncludeCompleted,
            ct: ct);

        return tasks
            .Select(t => new MyTaskDto(
                Id: t.Id,
                ProjectId: t.ProjectId,
                ListId: t.ListId,
                Title: t.Title,
                Priority: t.Priority,
                Type: t.Type,
                DisplayOrder: t.DisplayOrder,
                DueDate: t.DueDate,
                IsCompleted: t.IsCompleted,
                SubtasksTotal: t.Subtasks.Count,
                SubtasksCompleted: t.Subtasks.Count(s => s.IsCompleted)))
            .ToList()
            .AsReadOnly();
    }
}