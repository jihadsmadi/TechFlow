using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TechFlow.Domain.Tasks.TaskAssignments;

namespace TechFlow.Infrastructure.Persistence.Configurations;

public sealed class TaskAssignmentConfiguration : IEntityTypeConfiguration<TaskAssignment>
{
    public void Configure(EntityTypeBuilder<TaskAssignment> builder)
    {
        builder.ToTable("TaskAssignments");

        builder.HasKey(a => a.Id);

        builder.Property(a => a.TaskId).IsRequired();
        builder.Property(a => a.UserId).IsRequired();
        builder.Property(a => a.AssignedByUserId).IsRequired();
        builder.Property(a => a.AssignedAt).IsRequired();

        builder.HasIndex(a => a.TaskId);
        builder.HasIndex(a => new { a.TaskId, a.UserId }).IsUnique();
    }
}