using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TechFlow.Domain.Tasks.Subtasks;

namespace TechFlow.Infrastructure.Persistence.Configurations;

public sealed class SubtaskConfiguration : IEntityTypeConfiguration<Subtask>
{
    public void Configure(EntityTypeBuilder<Subtask> builder)
    {
        builder.ToTable("Subtasks");

        builder.HasKey(s => s.Id);

        builder.Property(s => s.TaskId).IsRequired();
        builder.Property(s => s.CreatedByUserId).IsRequired();

        builder.Property(s => s.Title)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(s => s.IsCompleted).IsRequired();
        builder.Property(s => s.CreatedAt).IsRequired();

        builder.HasIndex(s => s.TaskId);
    }
}