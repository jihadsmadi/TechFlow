using MediatR;
using TechFlow.Application.Common.Interfaces;
using TechFlow.Application.Features.Companies.Dtos;
using TechFlow.Domain.Common.Constants;
using TechFlow.Domain.Common.Results;

namespace TechFlow.Application.Features.Companies.Queries.GetAllCompanies
{
    public sealed record GetAllCompaniesQuery()
     : IRequest<Result<List<CompanyDto>>>, ICachedQuery
    {
        public string CacheKey => $"Companies:all";
        public string[] Tags => ["Companies"];
        public TimeSpan Expiration => TimeSpan.FromHours(TechFlowConstants.Cashe.CompanyExpirationHours);
    }
}
