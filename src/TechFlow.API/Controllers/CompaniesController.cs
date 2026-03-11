using MediatR;
using Microsoft.AspNetCore.Mvc;
using TechFlow.API.Authorization;
using TechFlow.API.Extensions;
using TechFlow.Application.Features.Companies.Commands.ActivateCompany;
using TechFlow.Application.Features.Companies.Commands.CreateCompany;
using TechFlow.Application.Features.Companies.Commands.DeactivateCompany;
using TechFlow.Application.Features.Companies.Commands.SetFeatureFlag;
using TechFlow.Application.Features.Companies.Commands.UpdateCompany;
using TechFlow.Application.Features.Companies.Commands.UpdateCompanySettings;
using TechFlow.Application.Features.Companies.Queries.GetAllCompanies;
using TechFlow.Application.Features.Companies.Queries.GetCompanyById;
using TechFlow.Application.Features.Companies.Queries.GetCompanyBySlug;
using TechFlow.Domain.Permissions.Const;

namespace TechFlow.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public sealed class CompaniesController(ISender sender) : ControllerBase
{
    private readonly ISender _sender = sender;
    // GET api/companies
    [HasPermission(PermissionNames.CompanyRead)]
    [HttpGet]
    public async Task<IActionResult> GetAll(CancellationToken ct)
    {
        var result = await _sender.Send(new GetAllCompaniesQuery(), ct);
        return result.ToActionResult(this);
    }

    // GET api/companies/{id}
    [HasPermission(PermissionNames.CompanyRead)]
    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id, CancellationToken ct)
    {
        var result = await _sender.Send(new GetCompanyByIdQuery(id), ct);
        return result.ToActionResult(this);
    }

    // GET api/companies/slug/{slug}
    [HasPermission(PermissionNames.CompanyRead)]
    [HttpGet("slug/{slug}")]
    public async Task<IActionResult> GetBySlug(string slug, CancellationToken ct)
    {
        var result = await _sender.Send(new GetCompanyBySlugQuery(slug), ct);
        return result.ToActionResult(this);
    }

    // POST api/companies
    [HasPermission(PermissionNames.CompanyCreate)] 
    [HttpPost]
    public async Task<IActionResult> Create(
        [FromBody] CreateCompanyCommand command,
        CancellationToken ct)
    {
        var result = await _sender.Send(command, ct);
        return result.ToActionResult(this);
    }

    // PUT api/companies/{id}
    [HasPermission(PermissionNames.CompanyUpdate)]
    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(
        Guid id,
        [FromBody] UpdateCompanyRequest request,
        CancellationToken ct)
    {
        var result = await _sender.Send(
            new UpdateCompanyCommand(id, request.Name, request.ContactEmail, request.Industry), ct);
        return result.ToNoContentResult(this);
    }


    // PUT api/companies/{id}/settings
    [HasPermission(PermissionNames.CompanyManageSettings)]
    [HttpPut("{id:guid}/settings")]
    public async Task<IActionResult> UpdateSettings(
        Guid id,
        [FromBody] UpdateCompanySettingsRequest request,
        CancellationToken ct)
    {
        var result = await _sender.Send(
            new UpdateCompanySettingsCommand(
                id,
                request.PrimaryColor,
                request.LogoUrl,
                request.CompanyWebsite,
                request.DefaultTimezone,
                request.DateFormat,
                request.Language,
                request.AllowGuestAccess,
                request.RequireTaskDueDate,
                request.AllowMembersInvite), ct);
        return result.ToNoContentResult(this);
    }

    // PATCH api/companies/{id}/features
    [HasPermission(PermissionNames.CompanyManageFlags)]
    [HttpPatch("{id:guid}/features")]
    public async Task<IActionResult> SetFeatureFlag(
        Guid id,
        [FromBody] SetFeatureFlagRequest request,
        CancellationToken ct)
    {
        var result = await _sender.Send(
            new SetFeatureFlagCommand(id, request.FeatureKey, request.IsEnabled, request.ToggledByUserId), ct);
        return result.ToNoContentResult(this);
    }

    // PATCH api/companies/{id}/activate
    [HasPermission(PermissionNames.CompanyManageSettings)] 
    [HttpPatch("{id:guid}/activate")]
    public async Task<IActionResult> Activate(Guid id, CancellationToken ct)
    {
        var result = await _sender.Send(new ActivateCompanyCommand(id), ct);
        return result.ToNoContentResult(this);
    }

    // PATCH api/companies/{id}/deactivate
    [HasPermission(PermissionNames.CompanyManageSettings)]
    [HttpPatch("{id:guid}/deactivate")]
    public async Task<IActionResult> Deactivate(Guid id, CancellationToken ct)
    {
        var result = await _sender.Send(new DeactivateCompanyCommand(id), ct);
        return result.ToNoContentResult(this);
    }
}

public sealed record UpdateCompanyRequest(
    string Name,
    string ContactEmail,
    string? Industry);

public sealed record UpdateCompanySettingsRequest(
    string PrimaryColor,
    string? LogoUrl,
    string? CompanyWebsite,
    string DefaultTimezone,
    string DateFormat,
    string Language,
    bool AllowGuestAccess,
    bool RequireTaskDueDate,
    bool AllowMembersInvite);

public sealed record SetFeatureFlagRequest(
    string FeatureKey,
    bool IsEnabled,
    Guid ToggledByUserId);