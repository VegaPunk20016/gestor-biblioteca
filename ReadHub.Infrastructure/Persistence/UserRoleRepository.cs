using ReadHub.Domain.Entities;
using ReadHub.Domain.Interfaces;
using ReadHub.Infrastructure.DataContext;

namespace ReadHub.Infrastructure.Persistence
{
    public class UserRoleRepository : IUserRoleRepository
    {
        private readonly AppDbContext _context;

        public UserRoleRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(UserRole userRole)
        {
            await _context.UserRoles.AddAsync(userRole);
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}