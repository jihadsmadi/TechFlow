using MediatR;
using TechFlow.Application.Features.Companies.Dtos;
using TechFlow.Domain.Common.Results;

namespace TechFlow.Application.Features.Companies.Commands.UpdateCompanySettings;

public sealed record UpdateCompanySettingsCommand(
    Guid CompanyId,
    string PrimaryColor,
    string? LogoUrl,
    string? CompanyWebsite,
    string DefaultTimezone,
    string DateFormat,
    string Language,
    bool AllowGuestAccess,
    bool RequireTaskDueDate,
    bool AllowMembersInvite
) : IRequest<Result<CompanyDto>>;