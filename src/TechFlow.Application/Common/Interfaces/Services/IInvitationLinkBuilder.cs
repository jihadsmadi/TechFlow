namespace TechFlow.Application.Common.Interfaces.Services;

public interface IInvitationLinkBuilder
{
    string BuildInvitationUrl(string token);
}