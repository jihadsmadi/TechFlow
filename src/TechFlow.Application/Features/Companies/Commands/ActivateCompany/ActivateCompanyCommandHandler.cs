using MediatR;
using Microsoft.Extensions.Logging;
using TechFlow.Application.Common.Interfaces.Repositories;
using TechFlow.Domain.Common.Results;
using TechFlow.Domain.Companies;

namespace TechFlow.Application.Features.Companies.Commands.ActivateCompany;

public sealed class ActivateCompanyCommandHandler(
    IUnitOfWork unitOfWork,
    ILogger<ActivateCompanyCommandHandler> logger)
    : IRequestHandler<ActivateCompanyCommand, Result<Updated>>
{
    public async Task<Result<Updated>> Handle(
        ActivateCompanyCommand command,
        CancellationToken ct)
    {
        var company = await unitOfWork.Companies.GetByIdAsync(command.Id, ct);
        if (company is null)
        {
            logger.LogWarning("Company not found: {Id}", command.Id);
            return CompanyErrors.NotFound;
        }

        // AlreadyActive guard runs inside domain
        var result = company.Activate();
        if (result.IsFailure)
        {
            logger.LogWarning("Company activate failed: {Errors}", result.Errors);
            return result.Errors;
        }

        unitOfWork.Companies.Update(company);
        await unitOfWork.SaveChangesAsync(ct);

        logger.LogInformation("Company activated: {Id}", company.Id);

        return Result.Updated;
    }
}