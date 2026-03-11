using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TechFlow.Domain.Projects;

namespace TechFlow.Infrastructure.Persistence.Configurations;

public sealed class ProjectMemberConfiguration : IEntityTypeConfiguration<ProjectMember>
{
    public void Configure(EntityTypeBuilder<ProjectMember> builder)
    {
        builder.ToTable("ProjectMembers");

        builder.HasKey(m => m.Id);

        builder.Property(m => m.ProjectId).IsRequired();
        builder.Property(m => m.UserId).IsRequired();
        builder.Property(m => m.AddedByUserId).IsRequired();
        builder.Property(m => m.AddedAt).IsRequired();

        // One user can only be a member of a project once
        builder.HasIndex(m => new { m.ProjectId, m.UserId })
            .IsUnique();

        builder.HasIndex(m => m.UserId);
    }
}
