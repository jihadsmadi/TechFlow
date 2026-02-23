using MediatR;
using TechFlow.Application.Common.Interfaces.Repositories;
using TechFlow.Application.Features.Permissions.DTOs;
using TechFlow.Application.Features.Permissions.Mappers;
using TechFlow.Domain.Common.Results;

namespace TechFlow.Application.Features.Permissions.Queries.GetAllPermissions;

public sealed class GetAllPermissionsQueryHandler(IUnitOfWork unitOfWork)
    : IRequestHandler<GetAllPermissionsQuery, Result<List<PermissionDto>>>
{
    public async Task<Result<List<PermissionDto>>> Handle(
        GetAllPermissionsQuery query,
        CancellationToken ct)
    {
        var permissions = query.Group is null
            ? await unitOfWork.Permissions.GetAllAsync(ct)
            : await unitOfWork.Permissions.GetByGroupAsync(query.Group, ct);

        return permissions.ToDtos();
    }
}