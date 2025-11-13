using Microsoft.EntityFrameworkCore;
using ReadHub.Libro.Domain.Entities;
using ReadHub.Libro.Domain.Interfaces;
using ReadHub.Libro.Infrastructure.DataContext;

namespace ReadHub.Libro.Infrastructure.Persistence
{
    public class LoanRepository : ILoanRepository
    {
        private readonly AppDbContext _context;
        public LoanRepository(AppDbContext context) => _context = context;

        public async Task AddAsync(Loan loan) =>
            await _context.Loans.AddAsync(loan);

        public async Task<Loan?> GetByIdAsync(Guid id) =>
            await _context.Loans
                .Include(l => l.Items)        
                .ThenInclude(i => i.Book)     
                .FirstOrDefaultAsync(l => l.Id == id);

        public async Task<IEnumerable<Loan>> GetActiveLoansAsync() =>
            await _context.Loans
                .Include(l => l.Items)
                .ThenInclude(i => i.Book)
                .ToListAsync();

        public async Task SaveChangesAsync() =>
            await _context.SaveChangesAsync();
        public async Task<LoanItem?> GetLoanItemByIdAsync(Guid loanItemId) =>
             await _context.LoanItems
                 .Include(i => i.Book)
                 .FirstOrDefaultAsync(i => i.Id == loanItemId);

    }
}
