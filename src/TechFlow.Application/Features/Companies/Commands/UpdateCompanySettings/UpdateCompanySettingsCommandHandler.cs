using MediatR;
using Microsoft.Extensions.Logging;
using TechFlow.Application.Common.Interfaces.Repositories;
using TechFlow.Application.Features.Companies.Dtos;
using TechFlow.Application.Features.Companies.Mappers;
using TechFlow.Domain.Common.Results;
using TechFlow.Domain.Companies;

namespace TechFlow.Application.Features.Companies.Commands.UpdateCompanySettings;

public sealed class UpdateCompanySettingsCommandHandler(
    IUnitOfWork unitOfWork,
    ILogger<UpdateCompanySettingsCommandHandler> logger)
    : IRequestHandler<UpdateCompanySettingsCommand, Result<CompanyDto>>
{
    public async Task<Result<CompanyDto>> Handle(
        UpdateCompanySettingsCommand command,
        CancellationToken ct)
    {
        var company = await unitOfWork.Companies.GetByIdAsync(command.CompanyId, ct);
        if (company is null)
        {
            logger.LogWarning("Company not found: {Id}", command.CompanyId);
            return CompanyErrors.NotFound;
        }

        var result = company.UpdateSettings(
            command.PrimaryColor,
            command.LogoUrl,
            command.CompanyWebsite,
            command.DefaultTimezone,
            command.DateFormat,
            command.Language,
            command.AllowGuestAccess,
            command.RequireTaskDueDate,
            command.AllowMembersInvite);

        if (result.IsFailure)
        {
            logger.LogWarning("Company settings update failed: {Errors}", result.Errors);
            return result.Errors;
        }

        unitOfWork.Companies.Update(company);
        await unitOfWork.SaveChangesAsync(ct);

        logger.LogInformation("Company settings updated: {Id}", company.Id);

        return company.ToDto();
    }
}