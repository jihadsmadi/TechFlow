using MediatR;
using TechFlow.Application.Common.Interfaces;
using System;
using TechFlow.Application.Features.Companies.Dtos;
using TechFlow.Domain.Common.Constants;
using TechFlow.Domain.Common.Results;

namespace TechFlow.Application.Features.Companies.Queries.GetCompanyById
{
    public sealed record GetCompanyByIdQuery(Guid Id)
     : IRequest<Result<CompanyDto>>, ICachedQuery
    {
        public string CacheKey => $"Companies:{Id}";
        public string[] Tags => ["Companies"];
        public TimeSpan Expiration => TimeSpan.FromHours(TechFlowConstants.Cashe.CompanyExpirationHours);
    }
}
