using Microsoft.EntityFrameworkCore;
using TechFlow.Application.Common.Interfaces.Repositories;
using TechFlow.Domain.Users;

namespace TechFlow.Infrastructure.Persistence.Repositories;

public sealed class UserRepository(ApplicationDbContext context) : IUserRepository
{
    private readonly ApplicationDbContext _context = context;

    public async Task<User?> GetByIdAsync(Guid id, CancellationToken ct = default)
        => await _context.Users
            .Include(u => u.UserCompanyRoles)
            .Include(u => u.UserProjectRoles)
            .FirstOrDefaultAsync(u => u.Id == id, ct);

    public async Task<User?> GetByIdentityIdAsync(
        Guid identityUserId, CancellationToken ct = default)
        => await _context.Users
            .Include(u => u.UserCompanyRoles)
            .Include(u => u.UserProjectRoles)
            .FirstOrDefaultAsync(u => u.IdentityUserId == identityUserId, ct);

    public async Task<User?> GetByEmailAsync(string email, CancellationToken ct = default)
        => await _context.Users
            .Include(u => u.UserCompanyRoles)
            .Include(u => u.UserProjectRoles)
            .FirstOrDefaultAsync(u => u.Email == email.ToLower(), ct);

    public async Task<bool> ExistsByEmailAsync(string email, CancellationToken ct = default)
        => await _context.Users
            .AnyAsync(u => u.Email == email.ToLower(), ct);

    public async Task<bool> IsFirstUserInCompanyAsync(
        Guid companyId, CancellationToken ct = default)
        => !await _context.Users
            .AnyAsync(u => u.CompanyId == companyId, ct);

    /// <summary>
    /// Single JOIN query — loads company-wide role names and all their permissions.
    /// No N+1. Used by Login, Refresh, Register to build JWT claims.
    /// </summary>
    public async Task<UserCompanyRolesDto> GetCompanyRolesAsync(
        Guid userId, CancellationToken ct = default)
    {
        var rows = await _context.UserCompanyRoles
            .Where(ucr => ucr.UserId == userId)
            .Join(
                _context.Roles.Include(r => r.Permissions),
                ucr  => ucr.RoleId,
                role => role.Id,
                (ucr, role) => role)
            .ToListAsync(ct);

        var roleNames   = rows.Select(r => r.Name).Distinct().ToList();
        var permissions = rows
            .SelectMany(r => r.Permissions.Select(p => p.Name))
            .Distinct()
            .ToList();

        return new UserCompanyRolesDto(roleNames, permissions);
    }

    public void Add(User user)
        => _context.Users.Add(user);
}
