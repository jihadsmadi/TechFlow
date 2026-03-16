using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TechFlow.API.Authorization;
using TechFlow.API.Extensions;
using TechFlow.Application.Features.Boards.Commands.AddList;
using TechFlow.Application.Features.Boards.Commands.RemoveList;
using TechFlow.Application.Features.Boards.Commands.RenameBoard;
using TechFlow.Application.Features.Boards.Commands.RenameList;
using TechFlow.Application.Features.Boards.Commands.ReorderLists;
using TechFlow.Application.Features.Boards.Commands.SetListColor;
using TechFlow.Application.Features.Boards.Queries.GetBoardByProjectId;
using TechFlow.Domain.Permissions.Const;

namespace TechFlow.API.Controllers;

[ApiController]
[Route("api/projects/{projectId:guid}/board")]
[Authorize]
public sealed class BoardsController(ISender _sender) : ControllerBase
{

    // GET api/projects/{projectId}/board
    [HasPermission(PermissionNames.ProjectsRead)]
    [HttpGet]
    public async Task<IActionResult> GetBoard(Guid projectId, CancellationToken ct)
    {
        var result = await _sender.Send(new GetBoardByProjectIdQuery(projectId), ct);
        return result.ToActionResult(this);
    }

    // PATCH api/projects/{projectId}/board/rename
    [HasPermission(PermissionNames.ProjectsUpdate)]
    [HttpPatch("rename")]
    public async Task<IActionResult> RenameBoard(
        Guid projectId,
        [FromBody] RenameBoardRequest request,
        CancellationToken ct)
    {
        var result = await _sender.Send(
            new RenameBoardCommand(projectId, request.Name), ct);
        return result.ToNoContentResult(this);
    }

    // POST api/projects/{projectId}/board/lists
    [HasPermission(PermissionNames.ProjectsUpdate)]
    [HttpPost("lists")]
    public async Task<IActionResult> AddList(
        Guid projectId,
        [FromBody] AddListRequest request,
        CancellationToken ct)
    {
        var result = await _sender.Send(
            new AddListCommand(projectId, request.Name, request.Color), ct);
        return result.ToNoContentResult(this);
    }

    // DELETE api/projects/{projectId}/board/lists/{listId}
    [HasPermission(PermissionNames.ProjectsUpdate)]
    [HttpDelete("lists/{listId:guid}")]
    public async Task<IActionResult> RemoveList(
        Guid projectId,
        Guid listId,
        CancellationToken ct)
    {
        var result = await _sender.Send(
            new RemoveListCommand(projectId, listId), ct);
        return result.ToNoContentResult(this);
    }

    // PATCH api/projects/{projectId}/board/lists/{listId}/rename
    [HasPermission(PermissionNames.ProjectsUpdate)]
    [HttpPatch("lists/{listId:guid}/rename")]
    public async Task<IActionResult> RenameList(
        Guid projectId,
        Guid listId,
        [FromBody] RenameListRequest request,
        CancellationToken ct)
    {
        var result = await _sender.Send(
            new RenameListCommand(projectId, listId, request.Name), ct);
        return result.ToNoContentResult(this);
    }

    // PATCH api/projects/{projectId}/board/lists/{listId}/color
    [HasPermission(PermissionNames.ProjectsUpdate)]
    [HttpPatch("lists/{listId:guid}/color")]
    public async Task<IActionResult> SetListColor(
        Guid projectId,
        Guid listId,
        [FromBody] SetListColorRequest request,
        CancellationToken ct)
    {
        var result = await _sender.Send(
            new SetListColorCommand(projectId, listId, request.Color), ct);
        return result.ToNoContentResult(this);
    }

    // PUT api/projects/{projectId}/board/lists/reorder
    [HasPermission(PermissionNames.ProjectsUpdate)]
    [HttpPut("lists/reorder")]
    public async Task<IActionResult> ReorderLists(
        Guid projectId,
        [FromBody] ReorderListsRequest request,
        CancellationToken ct)
    {
        var result = await _sender.Send(
            new ReorderListsCommand(projectId, request.OrderedListIds), ct);
        return result.ToNoContentResult(this);
    }
}

public sealed record RenameBoardRequest(string Name);
public sealed record AddListRequest(string Name, string? Color = null);
public sealed record RenameListRequest(string Name);
public sealed record SetListColorRequest(string? Color);
public sealed record ReorderListsRequest(List<Guid> OrderedListIds);
