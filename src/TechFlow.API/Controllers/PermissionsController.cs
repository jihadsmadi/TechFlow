using MediatR;
using Microsoft.AspNetCore.Mvc;
using TechFlow.API.Authorization;
using TechFlow.API.Extensions;
using TechFlow.Application.Features.Permissions.Commands.CreatePermission;
using TechFlow.Application.Features.Permissions.Commands.UpdatePermissionDescription;
using TechFlow.Application.Features.Permissions.Queries.GetAllPermissions;
using TechFlow.Application.Features.Permissions.Queries.GetPermissionById;
using TechFlow.Domain.Permissions.Const;

namespace TechFlow.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public sealed class PermissionsController(ISender sender) : ControllerBase
{
    private readonly ISender _sender = sender;

    // GET api/permissions?group=tasks
    [HasPermission(PermissionNames.PermissionsRead)]
    [HttpGet]
    public async Task<IActionResult> GetAll(
        [FromQuery] string? group,
        CancellationToken ct)
    {
        var result = await _sender.Send(new GetAllPermissionsQuery(group), ct);
        return result.ToActionResult(this);
    }

    // GET api/permissions/{id}
    [HasPermission(PermissionNames.PermissionsRead)]
    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id, CancellationToken ct)
    {
        var result = await _sender.Send(new GetPermissionByIdQuery(id), ct);
        return result.ToActionResult(this);
    }

    // POST api/permissions
    [HasPermission(PermissionNames.PermissionsCreate)]
    [HttpPost]
    public async Task<IActionResult> Create(
        [FromBody] CreatePermissionCommand command,
        CancellationToken ct)
    {
        var result = await _sender.Send(command, ct);
        return result.ToActionResult(this);
    }

    // PATCH api/permissions/{id}/description
    [HasPermission(PermissionNames.PermissionsUpdate)]
    [HttpPatch("{id:guid}/description")]
    public async Task<IActionResult> UpdateDescription(
        Guid id,
        [FromBody] UpdateDescriptionRequest request,
        CancellationToken ct)
    {
        var result = await _sender.Send(
            new UpdatePermissionDescriptionCommand(id, request.Description), ct);
        return result.ToNoContentResult(this);
    }
}

public sealed record UpdateDescriptionRequest(string Description);