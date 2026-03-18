using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TechFlow.API.Authorization;
using TechFlow.API.Extensions;
using TechFlow.Application.Features.Sprints.Commands.AddTaskToSprint;
using TechFlow.Application.Features.Sprints.Commands.CancelSprint;
using TechFlow.Application.Features.Sprints.Commands.CreateSprint;
using TechFlow.Application.Features.Sprints.Commands.EndSprint;
using TechFlow.Application.Features.Sprints.Commands.RemoveTaskFromSprint;
using TechFlow.Application.Features.Sprints.Commands.StartSprint;
using TechFlow.Application.Features.Sprints.Commands.UpdateSprint;
using TechFlow.Application.Features.Sprints.Queries.GetActiveSprints;
using TechFlow.Application.Features.Sprints.Queries.GetAllSprints;
using TechFlow.Application.Features.Sprints.Queries.GetBacklog;
using TechFlow.Application.Features.Sprints.Queries.GetSprintById;
using TechFlow.Domain.Permissions.Const;

namespace TechFlow.API.Controllers;

[Authorize]
[ApiController]
[Route("api/projects/{projectId:guid}/sprints")]
public sealed class SprintsController(ISender sender) : ControllerBase
{
    // ── Queries 

    [HttpGet]
    [HasPermission(PermissionNames.SprintsRead)]
    public async Task<IActionResult> GetAll(Guid projectId, CancellationToken ct)
    {
        var result = await sender.Send(new GetAllSprintsQuery(projectId), ct);
        return result.ToActionResult(this);
    }

    [HttpGet("active")]
    [HasPermission(PermissionNames.SprintsRead)]
    public async Task<IActionResult> GetActive(Guid projectId, CancellationToken ct)
    {
        var result = await sender.Send(new GetActiveSprintQuery(projectId), ct);
        return result.ToActionResult(this);
    }

    [HttpGet("{sprintId:guid}")]
    [HasPermission(PermissionNames.SprintsRead)]
    public async Task<IActionResult> GetById(Guid projectId, Guid sprintId, CancellationToken ct)
    {
        var result = await sender.Send(new GetSprintByIdQuery(projectId, sprintId), ct);
        return result.ToActionResult(this);
    }

    [HttpGet("backlog")]
    [HasPermission(PermissionNames.TasksRead)]
    public async Task<IActionResult> GetBacklog(Guid projectId, CancellationToken ct)
    {
        var result = await sender.Send(new GetBacklogQuery(projectId), ct);
        return result.ToActionResult(this);
    }


    [HttpPost]
    [HasPermission(PermissionNames.SprintsCreate)]
    public async Task<IActionResult> Create(
        Guid projectId,
        [FromBody] CreateSprintCommand command,
        CancellationToken ct)
    {
        var result = await sender.Send(command with { ProjectId = projectId }, ct);
        return result.IsFailure
            ? result.ToActionResult(this)
            : CreatedAtAction(nameof(GetById),
                new { projectId, sprintId = result.Value.Id },
                result.Value);
    }

    [HttpPut("{sprintId:guid}")]
    [HasPermission(PermissionNames.SprintsUpdate)]
    public async Task<IActionResult> Update(
        Guid projectId,
        Guid sprintId,
        [FromBody] UpdateSprintCommand command,
        CancellationToken ct)
    {
        var result = await sender.Send(command with { ProjectId = projectId, SprintId = sprintId }, ct);
        return result.ToNoContentResult(this);
    }

    [HttpPatch("{sprintId:guid}/start")]
    [HasPermission(PermissionNames.SprintsManage)]
    public async Task<IActionResult> Start(Guid projectId, Guid sprintId, CancellationToken ct)
    {
        var result = await sender.Send(new StartSprintCommand(projectId, sprintId), ct);
        return result.ToNoContentResult(this);
    }

    [HttpPatch("{sprintId:guid}/end")]
    [HasPermission(PermissionNames.SprintsManage)]
    public async Task<IActionResult> End(
        Guid projectId,
        Guid sprintId,
        [FromBody] EndSprintRequest request,
        CancellationToken ct)
    {
        var result = await sender.Send(
            new EndSprintCommand(projectId, sprintId, request.IncompleteTasksAction), ct);
        return result.ToNoContentResult(this);
    }

    [HttpPatch("{sprintId:guid}/cancel")]
    [HasPermission(PermissionNames.SprintsManage)]
    public async Task<IActionResult> Cancel(Guid projectId, Guid sprintId, CancellationToken ct)
    {
        var result = await sender.Send(new CancelSprintCommand(projectId, sprintId), ct);
        return result.ToNoContentResult(this);
    }

    // ── Tasks 

    [HttpPost("{sprintId:guid}/tasks/{taskId:guid}")]
    [HasPermission(PermissionNames.SprintsUpdate)]
    public async Task<IActionResult> AddTask(
        Guid projectId,
        Guid sprintId,
        Guid taskId,
        CancellationToken ct)
    {
        var result = await sender.Send(
            new AddTaskToSprintCommand(projectId, sprintId, taskId), ct);
        return result.ToNoContentResult(this);
    }

    [HttpDelete("{sprintId:guid}/tasks/{taskId:guid}")]
    [HasPermission(PermissionNames.SprintsUpdate)]
    public async Task<IActionResult> RemoveTask(
        Guid projectId,
        Guid sprintId,
        Guid taskId,
        CancellationToken ct)
    {
        var result = await sender.Send(
            new RemoveTaskFromSprintCommand(projectId, sprintId, taskId), ct);
        return result.ToNoContentResult(this);
    }
}

public sealed record EndSprintRequest(string IncompleteTasksAction);