using MediatR;
using TechFlow.Application.Common.Interfaces;
using TechFlow.Application.Features.Companies.Dtos;
using TechFlow.Domain.Common.Results;

namespace TechFlow.Application.Features.Companies.Queries.GetCompanyBySlug;

public sealed record GetCompanyBySlugQuery(string Slug)
    : IRequest<Result<CompanyDto>>, ICachedQuery
{
    public string CacheKey => $"companies:slug:{Slug}";
    public string[] Tags => ["companies"];
    public TimeSpan Expiration => TimeSpan.FromHours(1);
}