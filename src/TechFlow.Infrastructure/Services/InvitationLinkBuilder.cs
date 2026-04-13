using Microsoft.Extensions.Options;
using TechFlow.Application.Common.Interfaces.Services;
using TechFlow.Infrastructure.Settings;

namespace TechFlow.Infrastructure.Services;

public sealed class InvitationLinkBuilder(IOptions<ClientSettings> options) 
        : IInvitationLinkBuilder
{
    private readonly string _baseUrl = options.Value.WebBaseUrl;

    public string BuildInvitationUrl(string token)
    {
        return $"{_baseUrl}/accept-invitation?token={Uri.EscapeDataString(token)}";
    }
}