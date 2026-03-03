using MediatR;
using TechFlow.Application.Common.Constants;
using TechFlow.Application.Common.Interfaces;
using TechFlow.Application.Features.Companies.Dtos;
using TechFlow.Domain.Common.Results;

namespace TechFlow.Application.Features.Companies.Commands.SetFeatureFlag;

public sealed record SetFeatureFlagCommand(
    Guid CompanyId,
    string FeatureKey,
    bool IsEnabled,
    Guid ToggledByUserId
) : IRequest<Result<CompanyDto>>, ICacheInvalidator
{
    public string[] Tags => [CacheKeys.Companies.Tag];
}