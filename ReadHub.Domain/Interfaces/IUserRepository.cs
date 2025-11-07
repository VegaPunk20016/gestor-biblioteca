using ReadHub.Domain.Entities;

namespace ReadHub.Domain.Interfaces
{
    public interface IUserRepository
    {
        Task<User?> GetByEmailAsync(string email);
        Task<User?> GetByIdAsync(Guid id);
        Task AddAsync(User user);
        Task<IEnumerable<User>> GetUsersByRoleAsync(string roleName);

        Task SaveChangesAsync();
    }
}
