using ReadHub.Libro.Application.Dtos;
using ReadHub.Libro.Domain.Entities;
using ReadHub.Libro.Domain.Interfaces;

namespace ReadHub.Libro.Application.Services
{
    public class LoanService
    {
        private readonly IBookRepository _bookRepository;
        private readonly ILoanRepository _loanRepository;

        public LoanService(IBookRepository bookRepository, ILoanRepository loanRepository)
        {
            _bookRepository = bookRepository;
            _loanRepository = loanRepository;
        }

        public async Task<Loan> CreateLoanAsync(CreateLoanDto dto)
        {
            var loan = new Loan
            {
                Id = Guid.NewGuid(),
                UserId = dto.UserId,
                ReturnDate = DateTime.UtcNow.AddDays(dto.LoanDays)
            };

            foreach (var item in dto.Books)
            {
                var book = await _bookRepository.GetByIdAsync(item.BookId);
                if (book == null) throw new Exception($"Libro no encontrado: {item.BookId}");
                if (book.Stock < item.Quantity) throw new Exception($"No hay suficiente stock de: {book.Title}");

                book.Stock -= item.Quantity;
                await _bookRepository.UpdateAsync(book);

                loan.Items.Add(new LoanItem
                {
                    Id = Guid.NewGuid(),
                    BookId = item.BookId,
                    Quantity = item.Quantity
                });
            }

            await _loanRepository.AddAsync(loan);
            await _loanRepository.SaveChangesAsync();

            return loan;
        }


        public async Task<LoanItem?> ReturnLoanItemAsync(Guid loanItemId, int quantityToReturn)
        {
            var loanItem = await _loanRepository.GetLoanItemByIdAsync(loanItemId);
            if (loanItem == null) return null;

            if (quantityToReturn <= 0) return null;

            var quantityReturned = Math.Min(loanItem.Quantity, quantityToReturn);

            loanItem.Quantity -= quantityReturned;

            loanItem.ReturnedAt = DateTime.UtcNow;

            
            var book = await _bookRepository.GetByIdAsync(loanItem.BookId);
            if (book != null)
            {
                book.Stock += quantityReturned;
                await _bookRepository.UpdateAsync(book);
            }

            await _loanRepository.SaveChangesAsync();

            return loanItem; 
        }



    }
}
