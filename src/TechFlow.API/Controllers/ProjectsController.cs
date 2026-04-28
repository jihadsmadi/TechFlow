using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TechFlow.API.Authorization;
using TechFlow.API.Extensions;
using TechFlow.Application.Features.Projects.Commands.AddProjectMember;
using TechFlow.Application.Features.Projects.Commands.ArchiveProject;
using TechFlow.Application.Features.Projects.Commands.CreateProject;
using TechFlow.Application.Features.Projects.Commands.DeleteProject;
using TechFlow.Application.Features.Projects.Commands.RemoveProjectMember;
using TechFlow.Application.Features.Projects.Commands.RestoreProject;
using TechFlow.Application.Features.Projects.Commands.UpdateProject;
using TechFlow.Application.Features.Projects.Commands.UpdateProjectSettings;
using TechFlow.Application.Features.Projects.Queries.GetAllProjects;
using TechFlow.Application.Features.Projects.Queries.GetProjectById;
using TechFlow.Application.Features.Projects.Queries.GetProjectMembers;
using TechFlow.Domain.Permissions.Const;

namespace TechFlow.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public sealed class ProjectsController(ISender sender) : ControllerBase
{
    private readonly ISender _sender = sender;

    // GET api/projects?includeArchived=false
    [HasPermission(PermissionNames.ProjectsRead)]
    [HttpGet]
    public async Task<IActionResult> GetAll(
        [FromQuery] bool includeArchived = false,
        CancellationToken ct = default)
    {
        var result = await _sender.Send(new GetAllProjectsQuery(includeArchived), ct);
        return result.ToActionResult(this);
    }

    // GET api/projects/{id}
    [HasPermission(PermissionNames.ProjectsRead)]
    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id, CancellationToken ct)
    {
        var result = await _sender.Send(new GetProjectByIdQuery(id), ct);
        return result.ToActionResult(this);
    }

    // GET api/projects/{id}/members
    [HasPermission(PermissionNames.ProjectsRead)]
    [HttpGet("{id:guid}/members")]
    public async Task<IActionResult> GetMembers(Guid id, CancellationToken ct)
    {
        var result = await _sender.Send(new GetProjectMembersQuery(id), ct);
        return result.ToActionResult(this);
    }

    // POST api/projects
    [HasPermission(PermissionNames.ProjectsCreate)]
    [HttpPost]
    public async Task<IActionResult> Create(
        [FromBody] CreateProjectCommand command,
        CancellationToken ct)
    {
        var result = await _sender.Send(command, ct);

        if(result.IsFailure)
            return result.ToActionResult(this);

        return result.ToCreatedResult(this, nameof(GetById),
            new { id = result.Value.Id });
    }

    // PUT api/projects/{id}
    [HasPermission(PermissionNames.ProjectsUpdate)]
    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(
        Guid id,
        [FromBody] UpdateProjectRequest request,
        CancellationToken ct)
    {
        var result = await _sender.Send(
            new UpdateProjectCommand(
                id,
                request.Name,
                request.Description,
                request.Color,
                request.StartDate,
                request.EndDate), ct);
        return result.ToNoContentResult(this);
    }

    // PUT api/projects/{id}/settings
    [HasPermission(PermissionNames.ProjectsUpdate)]
    [HttpPut("{id:guid}/settings")]
    public async Task<IActionResult> UpdateSettings(
        Guid id,
        [FromBody] UpdateProjectSettingsCommand command,
        CancellationToken ct)
    {
        var result = await _sender.Send(command with { ProjectId = id }, ct);
        return result.ToNoContentResult(this);
    }

    // PATCH api/projects/{id}/archive
    [HasPermission(PermissionNames.ProjectsArchive)]
    [HttpPatch("{id:guid}/archive")]
    public async Task<IActionResult> Archive(Guid id, CancellationToken ct)
    {
        var result = await _sender.Send(new ArchiveProjectCommand(id), ct);
        return result.ToNoContentResult(this);
    }

    // PATCH api/projects/{id}/restore
    [HasPermission(PermissionNames.ProjectsArchive)]
    [HttpPatch("{id:guid}/restore")]
    public async Task<IActionResult> Restore(Guid id, CancellationToken ct)
    {
        var result = await _sender.Send(new RestoreProjectCommand(id), ct);
        return result.ToNoContentResult(this);
    }

    // DELETE api/projects/{id}
    [HasPermission(PermissionNames.ProjectsDelete)]
    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken ct)
    {
        var result = await _sender.Send(new DeleteProjectCommand(id), ct);
        return result.ToNoContentResult(this);
    }

    // POST api/projects/{id}/members
    [HasPermission(PermissionNames.ProjectsManageMembers)]
    [HttpPost("{id:guid}/members")]
    public async Task<IActionResult> AddMember(
        Guid id,
        [FromBody] AddMemberRequest request,
        CancellationToken ct)
    {
        var result = await _sender.Send(
            new AddProjectMemberCommand(id, request.UserId), ct);
        return result.ToNoContentResult(this);
    }

    // DELETE api/projects/{id}/members/{userId}
    [HasPermission(PermissionNames.ProjectsManageMembers)]
    [HttpDelete("{id:guid}/members/{userId:guid}")]
    public async Task<IActionResult> RemoveMember(
        Guid id,
        Guid userId,
        CancellationToken ct)
    {
        var result = await _sender.Send(
            new RemoveProjectMemberCommand(id, userId), ct);
        return result.ToNoContentResult(this);
    }
}

public sealed record UpdateProjectRequest(
    string Name,
    string? Description,
    string? Color,
    DateTimeOffset? StartDate,
    DateTimeOffset? EndDate);

public sealed record AddMemberRequest(Guid UserId);