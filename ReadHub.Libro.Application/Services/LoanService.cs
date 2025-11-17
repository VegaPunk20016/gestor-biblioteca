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


        //        PRESTAR UN LIBRO
        public async Task<Loan> CreateLoanAsync(CreateLoanDto dto)
        {
            
            var userLoans = await _loanRepository.GetUserLoansAsync(dto.UserId);

        
            if (UserHasOverdueLoans(userLoans))
                throw new InvalidOperationException("El usuario tiene préstamos vencidos. No se puede realizar un nuevo préstamo.");

           
            var loan = new Loan
            {
                Id = Guid.NewGuid(),
                UserId = dto.UserId,
                ReturnDate = DateTime.UtcNow.AddDays(dto.LoanDays)
            };


            foreach (var item in dto.Books)
            {
                var book = await _bookRepository.GetByIdAsync(item.BookId);
                if (book == null)
                    throw new Exception($"Libro no encontrado: {item.BookId}");

                if (book.Stock < item.Quantity)
                    throw new Exception($"No hay suficiente stock de: {book.Title}");

   
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

        //      DEVOLVER UN LIBRO
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

   
        //      RENOVAR PRÉSTAMO
        public async Task<bool> RenewLoanAsync(Guid loanId, int extraDays)
        {
            var loan = await _loanRepository.GetByIdAsync(loanId);
            if (loan == null)
                throw new Exception("El préstamo no existe.");

            if (loan.ReturnDate < DateTime.UtcNow)
                throw new Exception("No se puede renovar un préstamo vencido.");

            loan.ReturnDate = loan.ReturnDate.AddDays(extraDays);

            await _loanRepository.SaveChangesAsync();
            return true;
        }

        // Verificar si el usuario tiene préstamos vencidos
        private bool UserHasOverdueLoans(IEnumerable<Loan> loans)
        {
            var today = DateTime.UtcNow;

            return loans.Any(loan =>
                loan.ReturnDate < today &&           
                loan.Items.Any(i => !i.IsReturned)   
            );
        }

        //Todos los prestamos vencidos 
        public async Task<IEnumerable<OverdueLoanDto>> GetOverdueLoansAsync()
        {
            var overdueLoans = await _loanRepository.GetOverdueLoansAsync();

            return overdueLoans.Select(loan => new OverdueLoanDto
            {
                LoanId = loan.Id,
                UserId = loan.UserId,
                LoanDate = loan.LoanDate,
                ReturnDate = loan.ReturnDate,
                Items = loan.Items.Select(item => new OverdueLoanItemDto
                {
                    BookTitle = item.Book.Title,
                    Quantity = item.Quantity,
                    IsReturned = item.IsReturned
                }).ToList()
            });
        }

        //Prestamos vencidos por usuario 
        public async Task<IEnumerable<OverdueLoanDto>> GetUserOverdueLoansAsync(Guid userId)
        {
            var today = DateTime.UtcNow;

            var loans = await _loanRepository.GetUserLoansAsync(userId);

            var overdueLoans = loans
                .Where(l => l.ReturnDate < today && l.Items.Any(i => !i.IsReturned))
                .Select(loan => new OverdueLoanDto
                {
                    LoanId = loan.Id,
                    LoanDate = loan.LoanDate,
                    ReturnDate = loan.ReturnDate,
                    Items = loan.Items
                        .Where(i => !i.IsReturned)
                        .Select(i => new OverdueLoanItemDto
                        {
                            BookTitle = i.Book.Title,
                            Quantity = i.Quantity,
                            IsReturned = i.IsReturned
                        }).ToList()
                });

            return overdueLoans;
        }

    }
}
