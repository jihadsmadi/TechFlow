using MediatR;
using TechFlow.Application.Features.Auth.Dtos;
using TechFlow.Domain.Common.Results;

namespace TechFlow.Application.Features.Auth.Commands.Register;

public sealed record RegisterCommand(
    // ── Company info 
    string CompanyName,
    string CompanySlug,
    string CompanyEmail,
    string? Industry,
    // ── User info 
    string FirstName,
    string LastName,
    string Email,
    string Password
) : IRequest<Result<AuthResponse>>;