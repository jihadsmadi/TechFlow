using MediatR;
using Microsoft.AspNetCore.Mvc;
using TechFlow.API.Authorization;
using TechFlow.API.Extensions;
using TechFlow.Application.Features.Services.ClearingCache.Commands;
using TechFlow.Domain.Permissions.Const;

namespace TechFlow.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CacheController(ISender sender) : ControllerBase
    {
        private readonly ISender _sender = sender;

        [HttpPost("clear")]
        [HasPermission(PermissionNames.CompanyManageFlags)]
        public async Task<IActionResult> ClearCache([FromQuery]string ?cacheKey,CancellationToken ct)
        {
            var result = await _sender.Send(new ClearCacheCommand(cacheKey), ct);
            return result.ToActionResult(this);
        }
    }
}
