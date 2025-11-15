using Microsoft.EntityFrameworkCore;
using ReadHub.Libro.Domain.Entities;
using ReadHub.Libro.Domain.Interfaces;
using ReadHub.Libro.Infrastructure.DataContext;

namespace ReadHub.Libro.Infrastructure.Persistence
{
    public class FineRepository : IFineRepository
    {
        private readonly AppDbContext _context;

        public FineRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(Fine fine)
        {
            await _context.Fines.AddAsync(fine);
        }

        public async Task<IEnumerable<Fine>> GetAllAsync()
        {
            return await _context.Fines.ToListAsync();
        }

        public async Task<IEnumerable<Fine>> GetFinesByUser(Guid userId)
        {
            return await _context.Fines
                .Where(f => f.UserId == userId)
                .ToListAsync();
        }

        public async Task<IEnumerable<Fine>> GetFinesByLoan(Guid loanId)
        {
            return await _context.Fines
                .Where(f => f.LoanId == loanId)
                .ToListAsync();
        }

        public async Task<Fine?> GetByIdAsync(Guid id)
        {
            return await _context.Fines.FindAsync(id);
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }

    }
}
