using Microsoft.Extensions.Logging;
using TechFlow.Application.Common.Interfaces.Services;

namespace TechFlow.Infrastructure.Services;

// Development simulation — logs the invitation link to the console
// Replace with a real MailKit/SendGrid implementation for production
// The link printed here is what you'd paste in the browser to test accept flow
public sealed class ConsoleEmailService(ILogger<ConsoleEmailService> logger) : IEmailService
{
    public Task SendInvitationAsync(
        string toEmail,
        string invitedByName,
        string companyName,
        string acceptUrl,
        DateTimeOffset expiresAt,
        CancellationToken ct = default)
    {
        // In production this sends a real email
        // In development paste the URL from the console into your browser
        logger.LogInformation(
            """
            ============================================================
            📧  INVITATION EMAIL (SIMULATED)
            ------------------------------------------------------------
            To:          {Email}
            Invited by:  {InvitedBy}
            Company:     {Company}
            Expires:     {ExpiresAt:yyyy-MM-dd HH:mm} UTC
            ------------------------------------------------------------
            Accept URL:
            {AcceptUrl}
            ============================================================
            """,
            toEmail,
            invitedByName,
            companyName,
            expiresAt,
            acceptUrl);

        return Task.CompletedTask;
    }
}