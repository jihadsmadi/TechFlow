namespace TechFlow.Application.Common.Interfaces.Services;

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