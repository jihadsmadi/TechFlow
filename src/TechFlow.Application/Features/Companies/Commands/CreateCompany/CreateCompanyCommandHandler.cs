using MediatR;
using Microsoft.Extensions.Logging;
using TechFlow.Application.Common.Interfaces.Repositories;
using TechFlow.Application.Features.Companies.Dtos;
using TechFlow.Application.Features.Companies.Mappers;
using TechFlow.Domain.Common.Results;
using TechFlow.Domain.Companies;

namespace TechFlow.Application.Features.Companies.Commands.CreateCompany;

public sealed class CreateCompanyCommandHandler(
    IUnitOfWork unitOfWork,
    ILogger<CreateCompanyCommandHandler> logger)
    : IRequestHandler<CreateCompanyCommand, Result<CompanyDto>>
{
    public async Task<Result<CompanyDto>> Handle(
        CreateCompanyCommand command,
        CancellationToken ct)
    {
        var slugExists = await unitOfWork.Companies.ExistsBySlugAsync(command.Slug, ct);
        if (slugExists)
        {
            logger.LogWarning("Company slug already exists: {Slug}", command.Slug);
            return CompanyErrors.SlugAlreadyExists;
        }

        var result = Company.Create(
            command.Name,
            command.Slug,
            command.ContactEmail,
            command.Industry);

        if (result.IsFailure)
        {
            logger.LogWarning("Company creation failed: {Errors}", result.Errors);
            return result.Errors;
        }

        // ── 3. Persist ─────────────────────────────────────────────────────────
        unitOfWork.Companies.Add(result.Value);
        await unitOfWork.SaveChangesAsync(ct);

        logger.LogInformation("Company created: {Name} ({Slug})", result.Value.Name, result.Value.Slug.Value);

        return result.Value.ToDto();
    }
}