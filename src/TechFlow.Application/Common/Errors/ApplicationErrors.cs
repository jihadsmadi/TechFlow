using TechFlow.Domain.Common.Results;

namespace TechFlow.Application.Common.Errors;

/// <summary>
/// Application-level errors that don't belong to any specific domain entity.
/// Domain errors (PermissionErrors, RoleErrors etc.) live in the Domain layer.
/// These are cross-cutting concerns — auth, access, system errors.
/// </summary>
public static class ApplicationErrors
{
    // ── Authentication ─────────────────────────────────────────────────────────

    public static readonly Error InvalidCredentials = Error.Unauthorized(
        "Auth.InvalidCredentials",
        "Email or password is incorrect.");

    public static readonly Error UserNotFound = Error.NotFound(
        "Auth.UserNotFound",
        "User was not found.");

    public static readonly Error RefreshTokenExpired = Error.Unauthorized(
        "Auth.RefreshTokenExpired",
        "Refresh token has expired. Please sign in again.");

    public static readonly Error InvalidRefreshToken = Error.Unauthorized(
        "Auth.InvalidRefreshToken",
        "Refresh token is invalid.");

    // ── Authorization ──────────────────────────────────────────────────────────

    public static readonly Error Forbidden = Error.Forbidden(
        "Auth.Forbidden",
        "You do not have permission to perform this action.");

    public static readonly Error Unauthenticated = Error.Unauthorized(
        "Auth.Unauthenticated",
        "You must be signed in to perform this action.");

    public static readonly Error UserIdClaimMissing = Error.Unauthorized(
        "Auth.UserIdClaimMissing",
        "User ID could not be read from token.");

    // ── System ─────────────────────────────────────────────────────────────────

    public static readonly Error UnexpectedError = Error.Unexpected(
        "System.UnexpectedError",
        "An unexpected error occurred. Please try again.");
}