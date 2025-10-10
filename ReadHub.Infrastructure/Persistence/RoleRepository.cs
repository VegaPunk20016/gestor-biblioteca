using Microsoft.EntityFrameworkCore;
using ReadHub.Domain.Entities;
using ReadHub.Domain.Interfaces;
using ReadHub.Infrastructure.DataContext;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReadHub.Infrastructure.Persistence
{
    public class RoleRepository : IRoleRepository
    {
        private readonly AppDbContext _context;
        public RoleRepository(AppDbContext context) => _context = context;

        public async Task<Role?> GetByNameAsync(string name)
        {
            return await _context.Roles.FirstOrDefaultAsync(r => r.Name == name);
        }

        public async Task SaveChangesAsync() => await _context.SaveChangesAsync();
    }
}
