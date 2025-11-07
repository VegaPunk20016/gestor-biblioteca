using ReadHub.Libro.Domain.Entities;

namespace ReadHub.Libro.Domain.Interfaces
{
    public interface ILoanRepository
    {
        Task AddAsync(Loan loan);
        Task<Loan?> GetByIdAsync(Guid id);
        Task<IEnumerable<Loan>> GetActiveLoansAsync();
        Task<LoanItem?> GetLoanItemByIdAsync(Guid loanItemId);
        Task SaveChangesAsync();
    }
}
