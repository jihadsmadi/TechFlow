using Microsoft.EntityFrameworkCore;
using TechFlow.Application.Common.Interfaces.Repositories;
using TechFlow.Domain.Invitations;

namespace TechFlow.Infrastructure.Persistence.Repositories;

public sealed class InvitationRepository(ApplicationDbContext context)
    : Repository<Invitation>(context), IInvitationRepository
{
    public async Task<Invitation?> GetByTokenHashAsync(
        string tokenHash,
        CancellationToken ct = default)
        => await context.Invitations
            .FirstOrDefaultAsync(i => i.TokenHash == tokenHash, ct);

    public async Task<Invitation?> GetPendingByEmailAsync(
        Guid companyId,
        string email,
        CancellationToken ct = default)
        => await context.Invitations
            .FirstOrDefaultAsync(i =>
                i.CompanyId == companyId &&
                i.Email == email.ToLowerInvariant() &&
                !i.IsUsed &&
                !i.IsRevoked, ct);

    public async Task<List<Invitation>> GetPendingByCompanyAsync(
        Guid companyId,
        CancellationToken ct = default)
        => await context.Invitations
            .AsNoTracking()
            .Where(i =>
                i.CompanyId == companyId &&
                !i.IsUsed &&
                !i.IsRevoked)
            .OrderByDescending(i => i.CreatedAtUtc)
            .ToListAsync(ct);
}