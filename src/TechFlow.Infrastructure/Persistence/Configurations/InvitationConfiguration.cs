using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TechFlow.Domain.Invitations;

namespace TechFlow.Infrastructure.Persistence.Configurations;

public sealed class InvitationConfiguration : IEntityTypeConfiguration<Invitation>
{
    public void Configure(EntityTypeBuilder<Invitation> builder)
    {
        builder.ToTable("Invitations");

        builder.HasKey(i => i.Id);

        builder.Property(i => i.CompanyId).IsRequired();
        builder.Property(i => i.InvitedByUserId).IsRequired();
        builder.Property(i => i.RoleId).IsRequired();

        builder.Property(i => i.Email)
            .IsRequired()
            .HasMaxLength(256);

        builder.Property(i => i.ProjectId);

        builder.Property(i => i.Type)
            .IsRequired()
            .HasConversion<string>()  
            .HasMaxLength(20);

        builder.Property(i => i.TokenHash)
            .IsRequired()
            .HasMaxLength(64);

        builder.Property(i => i.ExpiresAt).IsRequired();
        builder.Property(i => i.IsUsed).IsRequired();
        builder.Property(i => i.UsedAt);
        builder.Property(i => i.IsRevoked).IsRequired();

        // ── Indexes
        builder.HasIndex(i => i.TokenHash).IsUnique();

        builder.HasIndex(i => new { i.CompanyId, i.IsUsed, i.IsRevoked });

        builder.HasIndex(i => new { i.CompanyId, i.Email, i.IsUsed, i.IsRevoked });
    }
}