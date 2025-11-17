using ReadHub.Libro.Domain.Entities;

namespace ReadHub.Libro.Domain.Interfaces
{
    public interface ILoanRenewalRequestRepository
    {
        Task AddAsync(LoanRenewalRequest request);
        Task<LoanRenewalRequest?> GetByIdAsync(Guid id);
        Task<IEnumerable<LoanRenewalRequest>> GetByUserAsync(Guid userId);
        Task<IEnumerable<LoanRenewalRequest>> GetPendingAsync();
        Task<IEnumerable<LoanRenewalRequest>> GetByLoanIdAsync(Guid loanId);
        Task SaveChangesAsync();
    }
}
