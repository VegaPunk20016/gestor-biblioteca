using ReadHub.Libro.Domain.Entities;

namespace ReadHub.Libro.Domain.Interfaces
{
    public interface IFineRepository
    {
        Task AddAsync(Fine fine);
        Task SaveChangesAsync();
        Task<IEnumerable<Fine>> GetFinesByUser(Guid userId);
        Task<IEnumerable<Fine>> GetFinesByLoan(Guid loanId);
        Task<Fine?> GetByIdAsync(Guid id);
        Task<IEnumerable<Fine>> GetAllAsync();

    }
}
