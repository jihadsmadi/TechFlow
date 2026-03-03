using MediatR;
using TechFlow.Application.Common.Constants;
using TechFlow.Application.Common.Interfaces;
using TechFlow.Application.Features.Companies.Dtos;
using TechFlow.Domain.Common.Results;

namespace TechFlow.Application.Features.Companies.Commands.CreateCompany;

public sealed record CreateCompanyCommand(
    string Name,
    string Slug,
    string ContactEmail,
    string? Industry
) : IRequest<Result<CompanyDto>>, ICacheInvalidator
{
    public string[] Tags => [CacheKeys.Companies.Tag];
}