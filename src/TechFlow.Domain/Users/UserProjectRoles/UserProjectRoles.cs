using System;
using System.Collections.Generic;
using System.Text;
using TechFlow.Domain.Common;
using TechFlow.Domain.Common.Results;

namespace TechFlow.Domain.Users.UserProjectRoles
{
    public sealed class UserProjectRole : Entity
    {
        public Guid UserId { get; private set; }
        public Guid ProjectId { get; private set; }
        public Guid RoleId { get; private set; }
        public Guid AssignedByUserId { get; private set; }
        public DateTimeOffset AssignedAt { get; private set; }

        private UserProjectRole() { }

        private UserProjectRole(Guid id, Guid userId,Guid projectId, Guid roleId, Guid assignedByUserId)
            : base(id)
        {
            UserId = userId;
            ProjectId = projectId;
            RoleId = roleId;
            AssignedByUserId = assignedByUserId;
            AssignedAt = DateTimeOffset.UtcNow;
        }

        // ── Factory 

        public static Result<UserProjectRole> Create(Guid userId, Guid projectId, Guid roleId, Guid assignedByUserId)
        {
            if (!IsValidId(userId))
                return UserProjectRoleErrors.UserIdRequired;

            if (!IsValidId(roleId))
                return UserProjectRoleErrors.RoleIdRequired;

            if (!IsValidId(projectId))
                return UserProjectRoleErrors.ProjectIdRequired;

            if (!IsValidId(assignedByUserId))
                return UserProjectRoleErrors.AssignedByRequired;

            return new UserProjectRole(Guid.NewGuid(), userId,projectId, roleId, assignedByUserId);
        }



        // ── Private Validation 

        private static bool IsValidId(Guid id) => id != Guid.Empty;
    }
}
