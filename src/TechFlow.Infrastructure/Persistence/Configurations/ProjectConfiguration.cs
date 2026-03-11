using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TechFlow.Domain.Projects;

namespace TechFlow.Infrastructure.Persistence.Configurations;

public sealed class ProjectConfiguration : IEntityTypeConfiguration<Project>
{
    public void Configure(EntityTypeBuilder<Project> builder)
    {
        builder.ToTable("Projects");

        builder.HasKey(p => p.Id);

        builder.Property(p => p.CompanyId)
            .IsRequired();

        builder.Property(p => p.CreatedByUserId)
            .IsRequired();

        builder.Property(p => p.Name)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(p => p.Description)
            .HasMaxLength(500);

        builder.Property(p => p.Status)
            .IsRequired()
            .HasMaxLength(20);

        // ProjectColor value object — stored as single column
        builder.Property(p => p.Color)
            .HasConversion(
                color => color.Value,
                value => TechFlow.Domain.Projects.ValueObjects.ProjectColor.Create(value).Value)
            .IsRequired()
            .HasMaxLength(7);

        builder.Property(p => p.StartDate);
        builder.Property(p => p.EndDate);
        builder.Property(p => p.IsArchived).IsRequired();
        builder.Property(p => p.ArchivedAt);

        // ProjectSetting — owned entity (same table)
        builder.OwnsOne(p => p.Settings, settings =>
        {
            settings.Property(s => s.DefaultListNames)
                .IsRequired()
                .HasMaxLength(500);

            settings.Property(s => s.DefaultTaskType)
                .IsRequired()
                .HasMaxLength(20);

            settings.Property(s => s.DefaultPriority)
                .IsRequired()
                .HasMaxLength(20);

            settings.Property(s => s.SprintLockOnStart).IsRequired();
            settings.Property(s => s.SprintDurationDays).IsRequired();
            settings.Property(s => s.IncompleteTasksAction)
                .IsRequired()
                .HasMaxLength(20);

            settings.Property(s => s.AutoAssignCreator).IsRequired();
            settings.Property(s => s.RequireEstimate).IsRequired();
            settings.Property(s => s.AllowSubtasks).IsRequired();
        });

        // Members navigation
        builder.HasMany(p => p.Members)
            .WithOne()
            .HasForeignKey(m => m.ProjectId)
            .OnDelete(DeleteBehavior.Cascade);

        // Indexes
        builder.HasIndex(p => p.CompanyId);
        builder.HasIndex(p => new { p.CompanyId, p.Name });
    }
}
