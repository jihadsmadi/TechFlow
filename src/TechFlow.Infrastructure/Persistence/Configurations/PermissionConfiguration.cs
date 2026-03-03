using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TechFlow.Domain.Common.Constants;
using TechFlow.Domain.Permissions;

namespace TechFlow.Infrastructure.Persistence.Configurations;

public sealed class PermissionConfiguration : IEntityTypeConfiguration<Permission>
{
    public void Configure(EntityTypeBuilder<Permission> builder)
    {
        builder.ToTable("Permissions");

        builder.HasKey(p => p.Id);

        builder.Property(p => p.Name)
            .IsRequired()
            .HasMaxLength(TechFlowConstants.Validation.MaxNameLength);

        builder.Property(p => p.Group)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(p => p.Description)
            .IsRequired()
            .HasMaxLength(TechFlowConstants.Validation.MaxDescriptionLength);

        // ── Indexes ────────────────────────────────────────────────────────────

        // Unique — permission names must be globally unique (e.g. "tasks.create")
        builder.HasIndex(p => p.Name)
            .IsUnique();

      
        builder.HasIndex(p => p.Group);
    }
}
