using Microsoft.EntityFrameworkCore;
using ReadHub.Libro.Domain.Entities;
using ReadHub.Libro.Domain.Interfaces;
using ReadHub.Libro.Infrastructure.DataContext;

namespace ReadHub.Libro.Infrastructure.Persistence
{
    public class PendingLoanRepository : IPendingLoanRepository
    {
        private readonly AppDbContext _context;

        public PendingLoanRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(PendingLoanBook pendingLoan)
        {
            await _context.PendingLoanBooks.AddAsync(pendingLoan);
        }

        public async Task<IEnumerable<PendingLoanBook>> GetAllAsync()
        {
            return await _context.PendingLoanBooks.ToListAsync();
        }

        public async Task<PendingLoanBook?> GetByIdAsync(Guid id)
        {
            return await _context.PendingLoanBooks.FindAsync(id);
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}
