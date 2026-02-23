using MediatR;
using TechFlow.Application.Common.Interfaces.Repositories;
using TechFlow.Application.Features.Permissions.DTOs;
using TechFlow.Application.Features.Permissions.Mappers;
using TechFlow.Domain.Common.Results;
using TechFlow.Domain.Permissions;

namespace TechFlow.Application.Features.Permissions.Queries.GetPermissionById;

public sealed class GetPermissionByIdQueryHandler(IUnitOfWork unitOfWork)
    : IRequestHandler<GetPermissionByIdQuery, Result<PermissionDto>>
{
    public async Task<Result<PermissionDto>> Handle(
        GetPermissionByIdQuery query,
        CancellationToken ct)
    {
        var permission = await unitOfWork.Permissions.GetByIdAsync(query.Id, ct);

        if (permission is null)
            return PermissionErrors.NotFound;

        return permission.ToDto();
    }
}