using MediatR;
using TechFlow.Application.Common.Interfaces.Repositories;
using TechFlow.Application.Features.Roles.DTOs;
using TechFlow.Application.Features.Roles.Mappers;
using TechFlow.Domain.Common.Results;

namespace TechFlow.Application.Features.Roles.Queries.GetAllRoles;

public sealed class GetAllRolesQueryHandler(IUnitOfWork unitOfWork)
    : IRequestHandler<GetAllRolesQuery, Result<List<RoleSummaryDto>>>
{
    public async Task<Result<List<RoleSummaryDto>>> Handle(
        GetAllRolesQuery query,
        CancellationToken ct)
    {
        var roles = await unitOfWork.Roles.GetAllAsync(ct);

        return roles.ToSummaryDtos();
    }
}