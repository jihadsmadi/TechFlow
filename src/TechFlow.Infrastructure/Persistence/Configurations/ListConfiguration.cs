using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TechFlow.Domain.Boards;

namespace TechFlow.Infrastructure.Persistence.Configurations;

public sealed class ListConfiguration : IEntityTypeConfiguration<List>
{
    public void Configure(EntityTypeBuilder<List> builder)
    {
        builder.ToTable("Lists");

        builder.HasKey(l => l.Id);

        builder.Property(l => l.BoardId).IsRequired();

        builder.Property(l => l.Name)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(l => l.Color)
            .HasMaxLength(7);

        builder.Property(l => l.DisplayOrder).IsRequired();
        builder.Property(l => l.IsDefault).IsRequired();
        builder.Property(l => l.IsCompletedList).IsRequired();

        // No duplicate list names on the same board
        builder.HasIndex(l => new { l.BoardId, l.Name })
            .IsUnique();

        builder.HasIndex(l => new { l.BoardId, l.DisplayOrder });
    }
}
