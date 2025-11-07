using ReadHub.Domain.Entities;

namespace ReadHub.Domain.Interfaces
{
    public interface IUserRoleRepository
    {
        Task AddAsync(UserRole userRole);
        Task RemoveRangeAsync(IEnumerable<UserRole> userRoles);
        Task SaveChangesAsync();
    }
}
