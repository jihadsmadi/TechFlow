using MediatR;
using Microsoft.Extensions.Logging;
using TechFlow.Application.Common.Interfaces.Repositories;
using TechFlow.Application.Features.Companies.Dtos;
using TechFlow.Application.Features.Companies.Mappers;
using TechFlow.Domain.Common.Results;
using TechFlow.Domain.Companies;

namespace TechFlow.Application.Features.Companies.Commands.UpdateCompany;

public sealed class UpdateCompanyCommandHandler(
    IUnitOfWork unitOfWork,
    ILogger<UpdateCompanyCommandHandler> logger)
    : IRequestHandler<UpdateCompanyCommand, Result<CompanyDto>>
{
    public async Task<Result<CompanyDto>> Handle(
        UpdateCompanyCommand command,
        CancellationToken ct)
    {
        var company = await unitOfWork.Companies.GetByIdAsync(command.Id, ct);
        if (company is null)
        {
            logger.LogWarning("Company not found: {Id}", command.Id);
            return CompanyErrors.NotFound;
        }

        var result = company.Update(command.Name, command.ContactEmail, command.Industry);
        if (result.IsFailure)
        {
            logger.LogWarning("Company update failed: {Errors}", result.Errors);
            return result.Errors;
        }

        unitOfWork.Companies.Update(company);
        await unitOfWork.SaveChangesAsync(ct);

        logger.LogInformation("Company updated: {Id}", company.Id);

        return company.ToDto();
    }
}