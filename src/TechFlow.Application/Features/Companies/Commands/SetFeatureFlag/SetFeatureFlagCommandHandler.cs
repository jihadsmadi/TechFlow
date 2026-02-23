using MediatR;
using Microsoft.Extensions.Logging;
using TechFlow.Application.Common.Interfaces.Repositories;
using TechFlow.Application.Features.Companies.Dtos;
using TechFlow.Application.Features.Companies.Mappers;
using TechFlow.Domain.Common.Results;
using TechFlow.Domain.Companies;

namespace TechFlow.Application.Features.Companies.Commands.SetFeatureFlag;

public sealed class SetFeatureFlagCommandHandler(
    IUnitOfWork unitOfWork,
    ILogger<SetFeatureFlagCommandHandler> logger)
    : IRequestHandler<SetFeatureFlagCommand, Result<CompanyDto>>
{
    public async Task<Result<CompanyDto>> Handle(
        SetFeatureFlagCommand command,
        CancellationToken ct)
    {
        var company = await unitOfWork.Companies.GetByIdAsync(command.CompanyId, ct);
        if (company is null)
        {
            logger.LogWarning("Company not found: {Id}", command.CompanyId);
            return CompanyErrors.NotFound;
        }

        var result = company.SetFeatureFlag(
            command.FeatureKey,
            command.IsEnabled,
            command.ToggledByUserId);

        if (result.IsFailure)
        {
            logger.LogWarning("Set feature flag failed: {Errors}", result.Errors);
            return result.Errors;
        }

        unitOfWork.Companies.Update(company);
        await unitOfWork.SaveChangesAsync(ct);

        logger.LogInformation(
            "Feature flag {FeatureKey} set to {IsEnabled} for company {Id}",
            command.FeatureKey, command.IsEnabled, company.Id);

        return company.ToDto();
    }
}