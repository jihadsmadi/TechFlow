using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TechFlow.Domain.Common.Constants;
using TechFlow.Domain.Users;

namespace TechFlow.Infrastructure.Persistence.Configurations;

public sealed class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.ToTable("Users");

        builder.HasKey(u => u.Id);

        builder.Property(u => u.IdentityUserId)
            .IsRequired();

        builder.Property(u => u.CompanyId)
            .IsRequired();

        builder.Property(u => u.FirstName)
            .IsRequired()
            .HasMaxLength(TechFlowConstants.Validation.MaxNameLength);

        builder.Property(u => u.LastName)
            .IsRequired()
            .HasMaxLength(TechFlowConstants.Validation.MaxNameLength);

        builder.Property(u => u.Email)
            .IsRequired()
            .HasMaxLength(TechFlowConstants.Validation.MaxEmailLength);

        builder.Property(u => u.AvatarUrl)
            .HasMaxLength(TechFlowConstants.Validation.UrlLength);

        builder.Property(u => u.IsActive)
            .IsRequired();

        // ── Audit fields 
        builder.Property(u => u.CreatedAtUtc).IsRequired();
        builder.Property(u => u.CreatedBy).HasMaxLength(50);
        builder.Property(u => u.LastModifiedUtc).IsRequired();
        builder.Property(u => u.LastModifiedBy).HasMaxLength(50);

        // ── Owned entity: UserPreferences → same table 
        builder.OwnsOne(u => u.Preferences, prefs =>
        {
            prefs.Property(p => p.Theme)
                .IsRequired()
                .HasMaxLength(20);

            prefs.Property(p => p.BoardView)
                .IsRequired()
                .HasMaxLength(20);

            prefs.Property(p => p.Language)
                .IsRequired()
                .HasMaxLength(5);

            prefs.Property(p => p.NotifyOnTaskAssigned).IsRequired();
            prefs.Property(p => p.NotifyOnCommentAdded).IsRequired();
            prefs.Property(p => p.NotifyOnDueDateNear).IsRequired();
            prefs.Property(p => p.NotifyOnTaskMoved).IsRequired();
            prefs.Property(p => p.NotifyOnMentioned).IsRequired();
            prefs.Property(p => p.DueDateReminderDays).IsRequired();
        });

        // ── Indexes 
        builder.HasIndex(u => u.IdentityUserId).IsUnique();
        builder.HasIndex(u => u.Email).IsUnique();
        builder.HasIndex(u => u.CompanyId);
        builder.HasIndex(u => u.IsActive);
    }
}