using MediatR;
using TechFlow.Application.Features.Companies.Dtos;
using TechFlow.Domain.Common.Results;

namespace TechFlow.Application.Features.Companies.Commands.SetFeatureFlag;

public sealed record SetFeatureFlagCommand(
    Guid CompanyId,
    string FeatureKey,
    bool IsEnabled,
    Guid ToggledByUserId
) : IRequest<Result<CompanyDto>>;