using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TechFlow.Domain.Common.Constants;
using TechFlow.Domain.Roles;

namespace TechFlow.Infrastructure.Persistence.Configurations;

public sealed class RoleConfiguration : IEntityTypeConfiguration<Role>
{
    public void Configure(EntityTypeBuilder<Role> builder)
    {
        builder.ToTable("Roles");

        builder.HasKey(r => r.Id);

        builder.Property(r => r.Name)
            .IsRequired()
            .HasMaxLength(TechFlowConstants.Validation.MaxNameLength);

        builder.Property(r => r.Description)
            .IsRequired()
            .HasMaxLength(TechFlowConstants.Validation.MaxDescriptionLength);

        builder.Property(r => r.IsSystemRole)
            .IsRequired();

        // ── Many-to-Many
        builder.HasMany(r => r.Permissions)
            .WithMany()
            .UsingEntity(j => j.ToTable("RolePermissions"));

        // ── Indexes 

        // Unique role names
        builder.HasIndex(r => r.Name)
            .IsUnique();

        // Frequently filtered — system roles excluded from delete/edit UI
        builder.HasIndex(r => r.IsSystemRole);
    }
}
