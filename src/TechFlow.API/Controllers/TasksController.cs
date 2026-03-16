using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TechFlow.API.Authorization;
using TechFlow.API.Extensions;
using TechFlow.Application.Features.Tasks.Commands.AddSubtask;
using TechFlow.Application.Features.Tasks.Commands.AssignTask;
using TechFlow.Application.Features.Tasks.Commands.CompleteSubTask;
using TechFlow.Application.Features.Tasks.Commands.CreateTask;
using TechFlow.Application.Features.Tasks.Commands.DeleteTask;
using TechFlow.Application.Features.Tasks.Commands.MoveTask;
using TechFlow.Application.Features.Tasks.Commands.RemoveSubtask;
using TechFlow.Application.Features.Tasks.Commands.RenameSubTask;
using TechFlow.Application.Features.Tasks.Commands.ReopenSubTask;
using TechFlow.Application.Features.Tasks.Commands.UnassignTask;
using TechFlow.Application.Features.Tasks.Commands.UpdateTask;
using TechFlow.Application.Features.Tasks.Queries.GetMyTasks;
using TechFlow.Application.Features.Tasks.Queries.GetTaskById;
using TechFlow.Application.Features.Tasks.Queries.GetTasksByList;
using TechFlow.Domain.Permissions.Const;

namespace TechFlow.API.Controllers;

[Authorize]
[ApiController]
[Route("api/projects/{projectId:guid}/tasks")]
public sealed class TasksController(ISender sender) : ControllerBase
{
    // ── Queries 

    [HttpGet("by-list/{listId:guid}")]
    [HasPermission(PermissionNames.TasksRead)]
    public async Task<IActionResult> GetTasksByList(
        Guid projectId,
        Guid listId,
        CancellationToken ct)
    {
        var result = await sender.Send(new GetTasksByListQuery(projectId, listId), ct);
        return result.ToActionResult(this);
    }

    [HttpGet("{taskId:guid}")]
    [HasPermission(PermissionNames.TasksRead)]
    public async Task<IActionResult> GetTaskById(
        Guid projectId,
        Guid taskId,
        CancellationToken ct)
    {
        var result = await sender.Send(new GetTaskByIdQuery(projectId, taskId), ct);
        return result.ToActionResult(this);
    }

    // ── Commands 

    [HttpPost]
    [HasPermission(PermissionNames.TasksCreate)]
    public async Task<IActionResult> CreateTask(
        Guid projectId,
        [FromBody] CreateTaskCommand command,
        CancellationToken ct)
    {
        var result = await sender.Send(command with { ProjectId = projectId }, ct);
        return result.IsFailure
            ? result.ToActionResult(this)
            : CreatedAtAction(nameof(GetTaskById), new { projectId, taskId = result.Value.Id }, result.Value);
    }

    [HttpPut("{taskId:guid}")]
    [HasPermission(PermissionNames.TasksUpdate)]
    public async Task<IActionResult> UpdateTask(
        Guid projectId,
        Guid taskId,
        [FromBody] UpdateTaskCommand command,
        CancellationToken ct)
    {
        var result = await sender.Send(command with { ProjectId = projectId, TaskId = taskId }, ct);
        return result.ToNoContentResult(this);
    }

    [HttpDelete("{taskId:guid}")]
    [HasPermission(PermissionNames.TasksDelete)]
    public async Task<IActionResult> DeleteTask(
        Guid projectId,
        Guid taskId,
        CancellationToken ct)
    {
        var result = await sender.Send(new DeleteTaskCommand(projectId, taskId), ct);
        return result.ToNoContentResult(this);
    }

    [HttpPatch("{taskId:guid}/move")]
    [HasPermission(PermissionNames.TasksMove)]
    public async Task<IActionResult> MoveTask(
        Guid projectId,
        Guid taskId,
        [FromBody] MoveTaskCommand command,
        CancellationToken ct)
    {
        var result = await sender.Send(command with { ProjectId = projectId, TaskId = taskId }, ct);
        return result.ToNoContentResult(this);
    }

    // ── Assignment 

    [HttpPost("{taskId:guid}/assignments/{userId:guid}")]
    [HasPermission(PermissionNames.TasksAssign)]
    public async Task<IActionResult> AssignTask(
        Guid projectId,
        Guid taskId,
        Guid userId,
        CancellationToken ct)
    {
        var result = await sender.Send(new AssignTaskCommand(projectId, taskId, userId), ct);
        return result.ToNoContentResult(this);
    }

    [HttpDelete("{taskId:guid}/assignments/{userId:guid}")]
    [HasPermission(PermissionNames.TasksAssign)]
    public async Task<IActionResult> UnassignTask(
        Guid projectId,
        Guid taskId,
        Guid userId,
        CancellationToken ct)
    {
        var result = await sender.Send(new UnassignTaskCommand(projectId, taskId, userId), ct);
        return result.ToNoContentResult(this);
    }

    // ── Subtasks 

    [HttpPost("{taskId:guid}/subtasks")]
    [HasPermission(PermissionNames.TasksUpdate)]
    public async Task<IActionResult> AddSubtask(
        Guid projectId,
        Guid taskId,
        [FromBody] AddSubtaskCommand command,
        CancellationToken ct)
    {
        var result = await sender.Send(command with { ProjectId = projectId, TaskId = taskId }, ct);
        return result.ToActionResult(this);
    }

    [HttpPatch("{taskId:guid}/subtasks/{subtaskId:guid}/rename")]
    [HasPermission(PermissionNames.TasksUpdate)]
    public async Task<IActionResult> RenameSubtask(
        Guid projectId,
        Guid taskId,
        Guid subtaskId,
        [FromBody] RenameSubtaskCommand command,
        CancellationToken ct)
    {
        var result = await sender.Send(
            command with { ProjectId = projectId, TaskId = taskId, SubtaskId = subtaskId }, ct);
        return result.ToNoContentResult(this);
    }

    [HttpPatch("{taskId:guid}/subtasks/{subtaskId:guid}/complete")]
    [HasPermission(PermissionNames.TasksUpdate)]
    public async Task<IActionResult> CompleteSubtask(
        Guid projectId,
        Guid taskId,
        Guid subtaskId,
        CancellationToken ct)
    {
        var result = await sender.Send(new CompleteSubtaskCommand(projectId, taskId, subtaskId), ct);
        return result.ToNoContentResult(this);
    }

    [HttpPatch("{taskId:guid}/subtasks/{subtaskId:guid}/reopen")]
    [HasPermission(PermissionNames.TasksUpdate)]
    public async Task<IActionResult> ReopenSubtask(
        Guid projectId,
        Guid taskId,
        Guid subtaskId,
        CancellationToken ct)
    {
        var result = await sender.Send(new ReopenSubtaskCommand(projectId, taskId, subtaskId), ct);
        return result.ToNoContentResult(this);
    }

    [HttpDelete("{taskId:guid}/subtasks/{subtaskId:guid}")]
    [HasPermission(PermissionNames.TasksDelete)]
    public async Task<IActionResult> RemoveSubtask(
        Guid projectId,
        Guid taskId,
        Guid subtaskId,
        CancellationToken ct)
    {
        var result = await sender.Send(new RemoveSubtaskCommand(projectId, taskId, subtaskId), ct);
        return result.ToNoContentResult(this);
    }
}