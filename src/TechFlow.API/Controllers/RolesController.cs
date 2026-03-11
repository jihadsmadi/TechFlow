using MediatR;
using Microsoft.AspNetCore.Mvc;
using TechFlow.API.Authorization;
using TechFlow.API.Extensions;
using TechFlow.Application.Features.Roles.Commands.CreateRole;
using TechFlow.Application.Features.Roles.Commands.DeleteRole;
using TechFlow.Application.Features.Roles.Commands.GrantPermission;
using TechFlow.Application.Features.Roles.Commands.RevokePermission;
using TechFlow.Application.Features.Roles.Commands.UpdateRole;
using TechFlow.Application.Features.Roles.Queries.GetAllRoles;
using TechFlow.Application.Features.Roles.Queries.GetRoleById;
using TechFlow.Domain.Permissions.Const;

namespace TechFlow.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public sealed class RolesController(ISender sender) : ControllerBase
{
    private readonly ISender _sender = sender;

    // GET api/roles
    [HasPermission(PermissionNames.RolesRead)]
    [HttpGet]
    public async Task<IActionResult> GetAll(CancellationToken ct)
    {
        var result = await _sender.Send(new GetAllRolesQuery(), ct);
        return result.ToActionResult(this);
    }

    // GET api/roles/{id}
    [HasPermission(PermissionNames.RolesRead)]
    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id, CancellationToken ct)
    {
        var result = await _sender.Send(new GetRoleByIdQuery(id), ct);
        return result.ToActionResult(this);
    }

    // POST api/roles
    [HasPermission(PermissionNames.RolesCreate)]
    [HttpPost]
    public async Task<IActionResult> Create(
        [FromBody] CreateRoleCommand command,
        CancellationToken ct)
    {
        var result = await _sender.Send(command, ct);
        return result.ToActionResult(this);
    }

    // PUT api/roles/{id}
    [HasPermission(PermissionNames.RolesUpdate)]
    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(
        Guid id,
        [FromBody] UpdateRoleRequest request,
        CancellationToken ct)
    {
        var result = await _sender.Send(
            new UpdateRoleCommand(id, request.Name, request.Description), ct);
        return result.ToNoContentResult(this);
    }

    // DELETE api/roles/{id}
    [HasPermission(PermissionNames.RolesDelete)]
    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken ct)
    {
        var result = await _sender.Send(new DeleteRoleCommand(id), ct);
        return result.ToNoContentResult(this);
    }

    // POST api/roles/{id}/permissions
    [HasPermission(PermissionNames.RolesAssignPermission)]
    [HttpPost("{id:guid}/permissions")]
    public async Task<IActionResult> GrantPermission(
        Guid id,
        [FromBody] PermissionIdRequest request,
        CancellationToken ct)
    {
        var result = await _sender.Send(
            new GrantPermissionCommand(id, request.PermissionId), ct);
        return result.ToNoContentResult(this);
    }

    // DELETE api/roles/{id}/permissions/{permissionId}
    [HasPermission(PermissionNames.RolesRevokePermission)]
    [HttpDelete("{id:guid}/permissions/{permissionId:guid}")]
    public async Task<IActionResult> RevokePermission(
        Guid id,
        Guid permissionId,
        CancellationToken ct)
    {
        var result = await _sender.Send(
            new RevokePermissionCommand(id, permissionId), ct);
        return result.ToNoContentResult(this);
    }
}

public sealed record UpdateRoleRequest(string Name, string Description);
public sealed record PermissionIdRequest(Guid PermissionId);