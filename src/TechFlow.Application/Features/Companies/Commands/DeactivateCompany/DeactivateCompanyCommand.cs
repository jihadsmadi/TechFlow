using MediatR;
using TechFlow.Application.Common.Constants;
using TechFlow.Application.Common.Interfaces;
using TechFlow.Domain.Common.Results;

namespace TechFlow.Application.Features.Companies.Commands.DeactivateCompany;

public sealed record DeactivateCompanyCommand(Guid Id) : IRequest<Result<Updated>>, ICacheInvalidator
{
    public string[] Tags => [CacheKeys.Companies.Tag];
}