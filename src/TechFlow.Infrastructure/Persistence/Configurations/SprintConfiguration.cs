using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TechFlow.Domain.Sprints;

namespace TechFlow.Infrastructure.Persistence.Configurations;

public sealed class SprintConfiguration : IEntityTypeConfiguration<Sprint>
{
    public void Configure(EntityTypeBuilder<Sprint> builder)
    {
        builder.ToTable("Sprints");

        builder.HasKey(s => s.Id);

        builder.Property(s => s.ProjectId).IsRequired();
        builder.Property(s => s.CompanyId).IsRequired();
        builder.Property(s => s.CreatedByUserId).IsRequired();

        builder.Property(s => s.SprintNumber).IsRequired();

        builder.Property(s => s.Name)
            .HasMaxLength(100);

        builder.Property(s => s.Goal)
            .HasMaxLength(500);

        builder.Property(s => s.Status)
            .IsRequired()
            .HasMaxLength(20);

        builder.Property(s => s.StartDate).IsRequired();
        builder.Property(s => s.EndDate).IsRequired();
        builder.Property(s => s.ActualEndDate);
        builder.Property(s => s.IsLocked).IsRequired();

        builder.HasMany(s => s.Items)
            .WithOne()
            .HasForeignKey(i => i.SprintId)
            .OnDelete(DeleteBehavior.Cascade);

        // unique sprint number per project
        builder.HasIndex(s => new { s.ProjectId, s.SprintNumber }).IsUnique();
        builder.HasIndex(s => s.ProjectId);
        builder.HasIndex(s => s.CompanyId);
        builder.HasIndex(s => new { s.ProjectId, s.Status });
    }
}