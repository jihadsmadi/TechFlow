using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TechFlow.API.Authorization;
using TechFlow.API.Extensions;
using TechFlow.Application.Features.Invitaions.Commands.InviteUser;
using TechFlow.Application.Features.Invitaions.Commands.RevokeInvitaion;
using TechFlow.Application.Features.Invitaions.Queries.GetPendingInvitations;
using TechFlow.Application.Features.Invitations.Commands.AcceptInvitation;
using TechFlow.Domain.Permissions.Const;

namespace TechFlow.API.Controllers;

[ApiController]
[Route("api/invitations")]
public sealed class InvitationsController(ISender sender) : ControllerBase
{

    // GET api/invitations
    [HttpGet]
    [Authorize]
    [HasPermission(PermissionNames.UsersInvite)]
    public async Task<IActionResult> GetPending(CancellationToken ct)
    {
        var result = await sender.Send(new GetPendingInvitationsQuery(), ct);
        return result.ToActionResult(this);
    }


    // POST api/invitations
    [HttpPost]
    [Authorize]
    [HasPermission(PermissionNames.UsersInvite)]
    public async Task<IActionResult> Invite(
        [FromBody] InviteUserCommand command,
        CancellationToken ct)
    {
        var result = await sender.Send(command, ct);
        return result.ToActionResult(this);
    }

    // POST api/invitations/accept
    [HttpPost("accept")]
    [AllowAnonymous]
    public async Task<IActionResult> Accept(
        [FromBody] AcceptInvitationCommand command,
        CancellationToken ct)
    {
        var result = await sender.Send(command, ct);
        return result.ToActionResult(this);
    }

    // DELETE api/invitations/{id}
    [HttpDelete("{id:guid}")]
    [Authorize]
    [HasPermission(PermissionNames.UsersInvite)]
    public async Task<IActionResult> Revoke(Guid id, CancellationToken ct)
    {
        var result = await sender.Send(new RevokeInvitationCommand(id), ct);
        return result.ToNoContentResult(this);
    }
}