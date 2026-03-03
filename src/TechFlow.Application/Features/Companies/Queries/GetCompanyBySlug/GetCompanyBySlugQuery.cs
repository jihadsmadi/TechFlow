using MediatR;
using TechFlow.Application.Common.Constants;
using TechFlow.Application.Common.Interfaces;
using TechFlow.Application.Features.Companies.Dtos;
using TechFlow.Domain.Common.Results;

namespace TechFlow.Application.Features.Companies.Queries.GetCompanyBySlug;

public sealed record GetCompanyBySlugQuery(string Slug)
    : IRequest<Result<CompanyDto>>, ICachedQuery
{
    public string CacheKey => CacheKeys.Companies.BySlug(Slug);
    public string[] Tags => [CacheKeys.Companies.Tag];
    public TimeSpan Expiration => TimeSpan.FromHours(CacheKeys.Companies.ExpirationHours);
}