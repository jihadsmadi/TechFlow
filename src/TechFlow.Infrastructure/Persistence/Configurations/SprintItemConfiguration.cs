using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TechFlow.Domain.Sprints;
using TechFlow.Domain.Sprints.ValueObjects;

namespace TechFlow.Infrastructure.Persistence.Configurations;

public sealed class SprintItemConfiguration : IEntityTypeConfiguration<SprintItem>
{
    public void Configure(EntityTypeBuilder<SprintItem> builder)
    {
        builder.ToTable("SprintItems");

        builder.HasKey(i => i.Id);

        builder.Property(i => i.SprintId).IsRequired();
        builder.Property(i => i.TaskId).IsRequired();
        builder.Property(i => i.AddedAt).IsRequired();

        builder.HasIndex(i => new { i.SprintId, i.TaskId }).IsUnique();
        builder.HasIndex(i => i.SprintId);
        builder.HasIndex(i => i.TaskId);
    }
}