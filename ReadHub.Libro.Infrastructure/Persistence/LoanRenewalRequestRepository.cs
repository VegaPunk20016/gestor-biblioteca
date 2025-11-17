using Microsoft.EntityFrameworkCore;
using ReadHub.Libro.Domain.Entities;
using ReadHub.Libro.Domain.Interfaces;
using ReadHub.Libro.Infrastructure.DataContext;

namespace ReadHub.Libro.Infrastructure.Persistence
{
    public class LoanRenewalRequestRepository : ILoanRenewalRequestRepository
    {
        private readonly AppDbContext _context;

        public LoanRenewalRequestRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(LoanRenewalRequest request)
        {
            await _context.LoanRenewalRequests.AddAsync(request);
        }

        public async Task<LoanRenewalRequest?> GetByIdAsync(Guid id)
        {
            return await _context.LoanRenewalRequests.FindAsync(id);
        }

        public async Task<IEnumerable<LoanRenewalRequest>> GetByUserAsync(Guid userId)
        {
            return await _context.LoanRenewalRequests
                .Where(r => r.UserId == userId)
                .ToListAsync();
        }

        public async Task<IEnumerable<LoanRenewalRequest>> GetPendingAsync()
        {
            return await _context.LoanRenewalRequests
                .Where(r => !r.IsApproved && !r.IsRejected)
                .ToListAsync();
        }
        public async Task<IEnumerable<LoanRenewalRequest>> GetByLoanIdAsync(Guid loanId)
        {
            return await _context.LoanRenewalRequests
                .Where(r => r.LoanId == loanId)
                .ToListAsync();
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
       
    }
}
