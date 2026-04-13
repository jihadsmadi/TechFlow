// ── TechFlow.Application/Common/Interfaces/Repositories/IInvitationRepository.cs

using TechFlow.Domain.Invitations;

namespace TechFlow.Application.Common.Interfaces.Repositories;

public interface IInvitationRepository : IRepository<Invitation>
{
    // Primary lookup — accept flow hashes the raw token and looks up by hash
    Task<Invitation?> GetByTokenHashAsync(string tokenHash, CancellationToken ct = default);

    // Find pending (not used, not revoked) invite for an email in a company
    // Used to revoke previous before creating new one
    Task<Invitation?> GetPendingByEmailAsync(Guid companyId, string email, CancellationToken ct = default);

    // List all pending invitations for a company — admin/PM view
    Task<List<Invitation>> GetPendingByCompanyAsync(Guid companyId, CancellationToken ct = default);
}