using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TechFlow.API.Extensions;
using TechFlow.Application.Common.Interfaces;
using TechFlow.Application.Features.Users.Queries.GetCurrentUser;

namespace TechFlow.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public sealed class UsersController(ISender sender, IUser currentUser) : ControllerBase
{
    private readonly ISender _sender = sender;
    private readonly IUser _currentUser = currentUser;

    // GET api/users/me
    [HttpGet("me")]
    public async Task<IActionResult> GetCurrentUser(CancellationToken ct)
    {
        if (_currentUser.Id is null)
            return Unauthorized();

        var result = await _sender.Send(
            new GetCurrentUserQuery(_currentUser.Id.Value), ct);

        return result.ToActionResult(this);
    }
}