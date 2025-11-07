using ReadHub.Libro.Domain.Entities;

namespace ReadHub.Libro.Domain.Interfaces
{
    public interface IBookRepository
    {
        Task<IEnumerable<Book>> GetAllAsync();
        Task<Book?> GetByIdAsync(Guid id);
        Task AddAsync(Book book);
        Task UpdateAsync(Book book);
        Task DeleteAsync(Book book);
        Task SaveChangesAsync();
        Task<Book?> GetByISBNAsync(string isbn);


    }
}
