using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace TechFlow.Infrastructure.Persistence.Configurations;

public sealed class TaskConfiguration : IEntityTypeConfiguration<Domain.Tasks.Task>
{
    public void Configure(EntityTypeBuilder<Domain.Tasks.Task> builder)
    {
        builder.ToTable("Tasks");

        builder.HasKey(t => t.Id);

        builder.Property(t => t.ListId).IsRequired();
        builder.Property(t => t.CompanyId).IsRequired();
        builder.Property(t => t.ProjectId).IsRequired();
        builder.Property(t => t.CreatedByUserId).IsRequired();

        builder.Property(t => t.Title)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(t => t.Description)
            .HasMaxLength(2000);

        builder.Property(t => t.Priority)
            .IsRequired()
            .HasMaxLength(20);

        builder.Property(t => t.Type)
            .IsRequired()
            .HasMaxLength(20);

        builder.Property(t => t.DisplayOrder)
            .IsRequired()
            .HasColumnType("float");

        builder.Property(t => t.DueDate);
        builder.Property(t => t.EstimatedMinutes);
        builder.Property(t => t.ActualMinutes);
        builder.Property(t => t.IsCompleted).IsRequired();
        builder.Property(t => t.CompletedAt);

        builder.HasMany(t => t.Subtasks)
            .WithOne()
            .HasForeignKey(s => s.TaskId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(t => t.Assignments)
            .WithOne()
            .HasForeignKey(a => a.TaskId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(t => t.ListId);
        builder.HasIndex(t => t.ProjectId);
        builder.HasIndex(t => t.CompanyId);
        builder.HasIndex(t => new { t.ListId, t.DisplayOrder });
        builder.HasIndex(t => new { t.ProjectId, t.IsCompleted });
    }
}