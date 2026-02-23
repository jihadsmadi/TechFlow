using MediatR;
using TechFlow.Application.Common.Interfaces.Repositories;
using TechFlow.Application.Features.Companies.Dtos;
using TechFlow.Application.Features.Companies.Mappers;
using TechFlow.Domain.Common.Results;
using TechFlow.Domain.Companies;

namespace TechFlow.Application.Features.Companies.Queries.GetCompanyBySlug;

public sealed class GetCompanyBySlugQueryHandler(IUnitOfWork unitOfWork)
    : IRequestHandler<GetCompanyBySlugQuery, Result<CompanyDto>>
{
    public async Task<Result<CompanyDto>> Handle(
        GetCompanyBySlugQuery query,
        CancellationToken ct)
    {
        var company = await unitOfWork.Companies.GetBySlugAsync(query.Slug, ct);

        if (company is null)
            return CompanyErrors.NotFound;

        return company.ToDto();
    }
}