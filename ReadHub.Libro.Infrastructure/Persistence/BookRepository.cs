using Microsoft.EntityFrameworkCore;
using ReadHub.Libro.Domain.Entities;
using ReadHub.Libro.Domain.Interfaces;
using ReadHub.Libro.Infrastructure.DataContext;

namespace ReadHub.Libro.Infrastructure.Persistence
{
    public class BookRepository: IBookRepository
    {
        private readonly AppDbContext _context;
        public BookRepository(AppDbContext context) => _context = context;

        public async Task<IEnumerable<Book>> GetAllAsync() => await _context.Books.ToListAsync();
        public async Task<Book?> GetByIdAsync(Guid id) => await _context.Books.FindAsync(id);
        public async Task AddAsync(Book book) => await _context.Books.AddAsync(book);
        public async Task UpdateAsync(Book book) => _context.Books.Update(book);
        public async Task DeleteAsync(Book book) => _context.Books.Remove(book);
        public async Task<Book?> GetByISBNAsync(string isbn)
        {
            return await _context.Books.FirstOrDefaultAsync(b => b.ISBN == isbn);
        }
        public async Task SaveChangesAsync() => await _context.SaveChangesAsync();

    }
}

