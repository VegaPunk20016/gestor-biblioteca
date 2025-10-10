using ReadHub.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReadHub.Domain.Interfaces
{
    public interface IUserRoleRepository
    {
        Task AddAsync(UserRole userRole);
        Task SaveChangesAsync();
    }
}
