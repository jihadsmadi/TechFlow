using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TechFlow.Domain.Users.UserCompanyRoles;

namespace TechFlow.Infrastructure.Persistence.Configurations;

public sealed class UserCompanyRoleConfiguration
    : IEntityTypeConfiguration<UserCompanyRole>
{
    public void Configure(EntityTypeBuilder<UserCompanyRole> builder)
    {
        builder.ToTable("UserCompanyRoles");

        builder.HasKey(r => r.Id);

        builder.Property(r => r.UserId)
            .IsRequired();

        builder.Property(r => r.RoleId)
            .IsRequired();

        builder.Property(r => r.AssignedByUserId)
            .IsRequired();

        builder.Property(r => r.AssignedAt)
            .IsRequired();

        builder.HasIndex(r => new { r.UserId, r.RoleId })
            .IsUnique();

        builder.HasOne<TechFlow.Domain.Users.User>()
            .WithMany(u => u.UserCompanyRoles)
            .HasForeignKey(r => r.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne<TechFlow.Domain.Roles.Role>()
            .WithMany()
            .HasForeignKey(r => r.RoleId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
