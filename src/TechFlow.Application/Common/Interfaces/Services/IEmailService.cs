namespace TechFlow.Application.Common.Interfaces.Services;

// Application layer defines the contract
// Infrastructure provides the implementation
// This keeps Application free of any email library dependency
public interface IEmailService
{
    Task SendInvitationAsync(
        string toEmail,
        string invitedByName,
        string companyName,
        string acceptUrl,       // full URL with raw token: https://app.techflow.dev/accept?token=xxx
        DateTimeOffset expiresAt,
        CancellationToken ct = default);
}