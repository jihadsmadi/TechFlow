using Microsoft.Extensions.Logging;
using TechFlow.Application.Common.Interfaces.Services;

namespace TechFlow.Infrastructure.Services;

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