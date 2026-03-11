using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using TechFlow.Application.Common.Interfaces;

namespace TechFlow.API.Services;

public sealed class CurrentUser(IHttpContextAccessor httpContextAccessor) : IUser
{
    private ClaimsPrincipal? User => httpContextAccessor.HttpContext?.User;

    public Guid? Id
    {
        get
        {
            var value = User?.FindFirstValue(JwtRegisteredClaimNames.Sub);
            return Guid.TryParse(value, out var id) ? id : null;
        }
    }

    public Guid CompanyId
    {
        get
        {
            var value = User?.FindFirstValue(Application.Common.Constants.ClaimNames.CompanyId);
            return Guid.TryParse(value, out var id) ? id : Guid.Empty;
        }
    }

    public bool IsInRole(string role) =>
        User?.IsInRole(role) ?? false;
}