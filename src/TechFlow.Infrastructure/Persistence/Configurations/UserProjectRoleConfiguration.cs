using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TechFlow.Domain.Users.UserProjectRoles;

namespace TechFlow.Infrastructure.Persistence.Configurations;

public sealed class UserProjectRoleConfiguration
    : IEntityTypeConfiguration<UserProjectRole>
{
    public void Configure(EntityTypeBuilder<UserProjectRole> builder)
    {
        builder.ToTable("UserProjectRoles");

        builder.HasKey(r => r.Id);

        builder.Property(r => r.UserId)
            .IsRequired();

        builder.Property(r => r.ProjectId)
            .IsRequired();

        builder.Property(r => r.RoleId)
            .IsRequired();

        builder.Property(r => r.AssignedByUserId)
            .IsRequired();

        builder.Property(r => r.AssignedAt)
            .IsRequired();

        builder.HasIndex(r => new { r.UserId, r.ProjectId, r.RoleId })
            .IsUnique();

        builder.HasOne<TechFlow.Domain.Users.User>()
            .WithMany(u => u.UserProjectRoles)
            .HasForeignKey(r => r.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne<TechFlow.Domain.Roles.Role>()
            .WithMany()
            .HasForeignKey(r => r.RoleId)
            .OnDelete(DeleteBehavior.Restrict);

    }
}
