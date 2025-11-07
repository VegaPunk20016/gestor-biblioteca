using ReadHub.Domain.Entities;

namespace ReadHub.Domain.Interfaces
{
    public interface IRoleRepository
    {
        Task<Role?> GetByNameAsync(string name);
        Task SaveChangesAsync();
    }
}
