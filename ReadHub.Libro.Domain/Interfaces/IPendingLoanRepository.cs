using ReadHub.Libro.Domain.Entities;

namespace ReadHub.Libro.Domain.Interfaces
{
    public interface IPendingLoanRepository
    {
        Task AddAsync(PendingLoanBook pendingLoan);
        Task<IEnumerable<PendingLoanBook>> GetAllAsync();
        Task<PendingLoanBook?> GetByIdAsync(Guid id);
        Task SaveChangesAsync();
    }
}
