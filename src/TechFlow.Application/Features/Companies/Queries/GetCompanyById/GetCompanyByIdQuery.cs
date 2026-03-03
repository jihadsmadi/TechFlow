using MediatR;
using System;
using TechFlow.Application.Common.Constants;
using TechFlow.Application.Common.Interfaces;
using TechFlow.Application.Features.Companies.Dtos;
using TechFlow.Domain.Common.Constants;
using TechFlow.Domain.Common.Results;

namespace TechFlow.Application.Features.Companies.Queries.GetCompanyById
{
    public sealed record GetCompanyByIdQuery(Guid Id)
     : IRequest<Result<CompanyDto>>, ICachedQuery
    {
        public string CacheKey => CacheKeys.Companies.ById(Id);
        public string[] Tags => [CacheKeys.Companies.Tag];
        public TimeSpan Expiration => TimeSpan.FromHours(CacheKeys.Companies.ExpirationHours);
    }
}
