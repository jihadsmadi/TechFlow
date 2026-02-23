using MediatR;
using TechFlow.Application.Common.Interfaces.Repositories;
using TechFlow.Application.Features.Roles.DTOs;
using TechFlow.Application.Features.Roles.Mappers;
using TechFlow.Domain.Common.Results;
using TechFlow.Domain.Roles;

namespace TechFlow.Application.Features.Roles.Queries.GetRoleById;

public sealed class GetRoleByIdQueryHandler(IUnitOfWork unitOfWork)
    : IRequestHandler<GetRoleByIdQuery, Result<RoleDto>>
{
    public async Task<Result<RoleDto>> Handle(
        GetRoleByIdQuery query,
        CancellationToken ct)
    {
        var role = await unitOfWork.Roles.GetWithPermissionsAsync(query.Id, ct);

        if (role is null)
            return RoleErrors.NotFound;

        return role.ToDto();
    }
}