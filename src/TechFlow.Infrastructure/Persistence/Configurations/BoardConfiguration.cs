using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TechFlow.Domain.Boards;

namespace TechFlow.Infrastructure.Persistence.Configurations;

public sealed class BoardConfiguration : IEntityTypeConfiguration<Board>
{
    public void Configure(EntityTypeBuilder<Board> builder)
    {
        builder.ToTable("Boards");

        builder.HasKey(b => b.Id);

        builder.Property(b => b.ProjectId).IsRequired();

        builder.Property(b => b.Name)
            .IsRequired()
            .HasMaxLength(100);

        // 1-to-1 with Project — one project, one board
        builder.HasIndex(b => b.ProjectId)
            .IsUnique();

        // Lists navigation
        builder.HasMany(b => b.Lists)
            .WithOne()
            .HasForeignKey(l => l.BoardId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
