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
            .HasConversion<string>()  // store as "Company" / "Project"
            .HasMaxLength(20);

        // TokenHash — SHA-256 hex string = 64 chars
        // indexed for fast lookup on accept
        builder.Property(i => i.TokenHash)
            .IsRequired()
            .HasMaxLength(64);

        builder.Property(i => i.ExpiresAt).IsRequired();
        builder.Property(i => i.IsUsed).IsRequired();
        builder.Property(i => i.UsedAt);
        builder.Property(i => i.IsRevoked).IsRequired();

        // ── Indexes
        // Primary lookup — accept by token hash
        builder.HasIndex(i => i.TokenHash).IsUnique();

        // Find pending invites for a company — used in GetPendingInvitations
        builder.HasIndex(i => new { i.CompanyId, i.IsUsed, i.IsRevoked });

        // Find pending invite for a specific email in a company
        // Used in duplicate check before creating new invite
        builder.HasIndex(i => new { i.CompanyId, i.Email, i.IsUsed, i.IsRevoked });
    }
}