using TechFlow.Domain.Users;

namespace TechFlow.Application.Common.Interfaces.Repositories;

public interface IUserRepository
{
    Task<User?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<User?> GetByIdentityIdAsync(Guid identityUserId, CancellationToken ct = default);
    Task<User?> GetByEmailAsync(string email, CancellationToken ct = default);
    Task<bool>  ExistsByEmailAsync(string email, CancellationToken ct = default);
    Task<bool>  IsFirstUserInCompanyAsync(Guid companyId, CancellationToken ct = default);

    Task<UserCompanyRolesDto> GetCompanyRolesAsync(Guid userId, CancellationToken ct = default);

    void Add(User user);
}

public sealed record UserCompanyRolesDto(
    IReadOnlyList<string> RoleNames,
    IReadOnlyList<string> Permissions);
