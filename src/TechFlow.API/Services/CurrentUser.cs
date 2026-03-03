using System.Security.Claims;
using TechFlow.Application.Common.Interfaces;

namespace TechFlow.API.Services;

public sealed class CurrentUser(IHttpContextAccessor httpContextAccessor) : IUser
{
    public Guid? Id
    {
        get
        {
            var value = httpContextAccessor.HttpContext?
                .User
                .FindFirstValue(ClaimTypes.NameIdentifier);

            return Guid.TryParse(value, out var id) ? id : null;
        }
    }
}