using ReadHub.Libro.Domain.Entities;
using ReadHub.Libro.Domain.Interfaces;
using ReadHub.Libro.Application.Dtos;

namespace ReadHub.Libro.Application.Services
{
    public class BookService
    {
        private readonly IBookRepository _bookRepository;
        public BookService(IBookRepository bookRepository)
        {
            _bookRepository = bookRepository;
        }


        //Consultar libros
        public async Task<IEnumerable<Book>> GetAllBooksAsync() => await _bookRepository.GetAllAsync();

        //Consultar libros por id
        public async Task<Book?> GetBookByIdAsync(Guid id) => await _bookRepository.GetByIdAsync(id);

        // Crear libro
        public async Task<Book> CreateBookAsync(BookDto bookDto)
        {
            
            var existingBook = await _bookRepository.GetByISBNAsync(bookDto.ISBN);
            if (existingBook != null)
                throw new Exception("El ISBN ya está registrado.");

            var book = new Book
            {
                Id = Guid.NewGuid(),
                Title = bookDto.Title,
                Author = bookDto.Author,
                Editorial = bookDto.Editorial,
                Year = bookDto.Year,
                Category = bookDto.Category,
                ISBN = bookDto.ISBN,
                Stock = bookDto.Stock
            };

            await _bookRepository.AddAsync(book);
            await _bookRepository.SaveChangesAsync();
            return book;
        }


        // Actualizar libro
        public async Task<Book?> UpdateBookAsync(Guid id, BookDto bookDto)
        {
            var existing = await _bookRepository.GetByIdAsync(id);
            if (existing == null) return null;

            existing.Title = bookDto.Title;
            existing.Author = bookDto.Author;
            existing.Editorial = bookDto.Editorial;
            existing.Year = bookDto.Year;
            existing.Category = bookDto.Category;
            existing.ISBN = bookDto.ISBN;
            existing.Stock = bookDto.Stock;

            await _bookRepository.UpdateAsync(existing);
            await _bookRepository.SaveChangesAsync();
            return existing;
        }

        // Eliminar libro
        public async Task<bool> DeleteBookAsync(Guid id)
        {
            var existing = await _bookRepository.GetByIdAsync(id);
            if (existing == null) return false;

            await _bookRepository.DeleteAsync(existing);
            await _bookRepository.SaveChangesAsync();
            return true;
        }
    }
}
