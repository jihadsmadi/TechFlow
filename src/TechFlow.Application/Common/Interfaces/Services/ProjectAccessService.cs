using TechFlow.Domain.Projects;

namespace TechFlow.Application.Common.Services;

public sealed class ProjectAccessService
{
    public bool CanAccess(Project project, Guid userId, bool isAdmin)
    => isAdmin || project.IsMember(userId);

    public bool CanModify(Project project, Guid userId, bool isAdmin)
        => isAdmin || project.IsCreator(userId);

    public bool CanManageMembers(Project project, Guid userId, bool isAdmin)
        => isAdmin || project.IsCreator(userId);
}
