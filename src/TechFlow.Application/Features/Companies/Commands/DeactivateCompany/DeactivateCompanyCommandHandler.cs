using MediatR;
using Microsoft.Extensions.Logging;
using TechFlow.Application.Common.Interfaces.Repositories;
using TechFlow.Domain.Common.Results;
using TechFlow.Domain.Companies;

namespace TechFlow.Application.Features.Companies.Commands.DeactivateCompany;

public sealed class DeactivateCompanyCommandHandler(
    IUnitOfWork unitOfWork,
    ILogger<DeactivateCompanyCommandHandler> logger)
    : IRequestHandler<DeactivateCompanyCommand, Result<Updated>>
{
    public async Task<Result<Updated>> Handle(
        DeactivateCompanyCommand command,
        CancellationToken ct)
    {
        var company = await unitOfWork.Companies.GetByIdAsync(command.Id, ct);
        if (company is null)
        {
            logger.LogWarning("Company not found: {Id}", command.Id);
            return CompanyErrors.NotFound;
        }

        // AlreadyInactive guard runs inside domain
        var result = company.Deactivate();
        if (result.IsFailure)
        {
            logger.LogWarning("Company deactivate failed: {Errors}", result.Errors);
            return result.Errors;
        }

        unitOfWork.Companies.Update(company);
        await unitOfWork.SaveChangesAsync(ct);

        logger.LogInformation("Company deactivated: {Id}", company.Id);

        return Result.Updated;
    }
}