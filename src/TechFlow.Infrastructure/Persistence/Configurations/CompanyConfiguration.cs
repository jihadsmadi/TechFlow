using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TechFlow.Domain.Common.Constants;
using TechFlow.Domain.Companies;
using TechFlow.Domain.Companies.ValueObjects;

namespace TechFlow.Infrastructure.Persistence.Configurations;

public sealed class CompanyConfiguration : IEntityTypeConfiguration<Company>
{
    public void Configure(EntityTypeBuilder<Company> builder)
    {
        builder.ToTable("Companies");

        builder.HasKey(c => c.Id);

        builder.Property(c => c.Name)
            .IsRequired()
            .HasMaxLength(TechFlowConstants.Validation.MaxNameLength);

        //// ── Slug value object → single string column via HasConversion ──────────
        //builder.Property(c => c.Slug)
        //    .IsRequired()
        //    .HasMaxLength(60)
        //    .HasConversion(
        //        slug => slug.Value,                  // write: Slug → string
        //        value => Slug.FromPersistence(value));  // read: string → Slug

        builder.Property(c => c.ContactEmail)
            .IsRequired()
            .HasMaxLength(TechFlowConstants.Validation.MaxEmailLength);

        builder.Property(c => c.Industry)
            .HasMaxLength(TechFlowConstants.Validation.MaxNameLength);

        builder.Property(c => c.IsActive)
            .IsRequired();

        // ── Audit fields 
        builder.Property(c => c.CreatedAtUtc).IsRequired();
        builder.Property(c => c.CreatedBy).HasMaxLength(50);
        builder.Property(c => c.LastModifiedUtc).IsRequired();
        builder.Property(c => c.LastModifiedBy).HasMaxLength(50);

        // ── Owned entity: 
        builder.OwnsOne(c => c.Slug, slug =>
        {
            slug.Property(s => s.Value)
                .HasColumnName("Slug")
                .IsRequired()
                .HasMaxLength(60);

            slug.HasIndex(s => s.Value)
                .IsUnique();
        });

        builder.OwnsOne(c => c.Settings, settings =>
        {
            settings.ToTable("CompanySettings");

            settings.Property(s => s.PrimaryColor)
                .IsRequired()
                .HasMaxLength(7);  

            settings.Property(s => s.LogoUrl)
                .HasMaxLength(TechFlowConstants.Validation.UrlLength);

            settings.Property(s => s.CompanyWebsite)
                .HasMaxLength(TechFlowConstants.Validation.UrlLength);

            settings.Property(s => s.DefaultTimezone)
                .IsRequired()
                .HasMaxLength(100);

            settings.Property(s => s.DateFormat)
                .IsRequired()
                .HasMaxLength(20);

            settings.Property(s => s.Language)
                .IsRequired()
                .HasMaxLength(5);

            settings.Property(s => s.AllowGuestAccess).IsRequired();
            settings.Property(s => s.RequireTaskDueDate).IsRequired();
            settings.Property(s => s.AllowMembersInvite).IsRequired();
        });

        // ── Owned collection: FeatureFlags → separate table
        builder.OwnsMany(c => c.FeatureFlags, ff =>
        {
            ff.ToTable("CompanyFeatureFlags");

            ff.WithOwner().HasForeignKey("CompanyId");

            // FeatureKey as part of composite key — one flag per key per company
            ff.HasKey("CompanyId", "FeatureKey");

            ff.Property(f => f.FeatureKey)
                .IsRequired()
                .HasMaxLength(50);

            ff.Property(f => f.IsEnabled).IsRequired();

            ff.Property(f => f.EnabledAt);

            ff.Property(f => f.EnabledByUserId);

            ff.Property(f => f.UpdatedAt).IsRequired();

            ff.HasIndex(f => f.FeatureKey);
        });

        // ── Indexes 

        builder.HasIndex(c => c.ContactEmail)
            .IsUnique();

        builder.HasIndex(c => c.IsActive);
    }
}
