using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using TechFlow.API.Extensions;
using TechFlow.Application.Features.Auth.Commands.Login;
using TechFlow.Application.Features.Auth.Commands.RefreshToken;
using TechFlow.Application.Features.Auth.Commands.Register;

namespace TechFlow.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public sealed class AuthController(ISender sender) : ControllerBase
{
    private readonly ISender _sender = sender;

    // POST api/auth/register
    [HttpPost("register")]
    [AllowAnonymous]
    public async Task<IActionResult> Register(
        [FromBody] RegisterCommand command,
        CancellationToken ct)
    {
        var result = await _sender.Send(command, ct);
        return result.ToActionResult(this);
    }

    // POST api/auth/login
    [HttpPost("login")]
    [AllowAnonymous]
    public async Task<IActionResult> Login(
        [FromBody] LoginCommand command,
        CancellationToken ct)
    {
        var result = await _sender.Send(command, ct);
        return result.ToActionResult(this);
    }

    // POST api/auth/refresh
    [HttpPost("refresh")]
    [AllowAnonymous]
    public async Task<IActionResult> Refresh(
        [FromBody] RefreshRequest request,
        CancellationToken ct)
    {
        var result = await _sender.Send(
            new RefreshTokenCommand(request.AccessToken, request.RefreshToken), ct);
        return result.ToActionResult(this);
    }

    [HttpGet("token-info")]
    [Authorize]
    public IActionResult GetTokenInfo()
    {
        var accessTokenExpiry = User.FindFirstValue(JwtRegisteredClaimNames.Exp);
        var expiry = DateTimeOffset.FromUnixTimeSeconds(long.Parse(accessTokenExpiry!));

        return Ok(new
        {
            accessToken = new
            {
                expiresAt = expiry.UtcDateTime,
                expiresIn = (int)(expiry - DateTimeOffset.UtcNow).TotalSeconds,
                isExpired = expiry < DateTimeOffset.UtcNow
            }
        });
    }
}

public sealed record RefreshRequest(string AccessToken, string RefreshToken);