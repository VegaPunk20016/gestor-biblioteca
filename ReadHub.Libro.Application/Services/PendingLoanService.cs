using ReadHub.Libro.Application.Dtos;
using ReadHub.Libro.Domain.Entities;
using ReadHub.Libro.Domain.Interfaces;

namespace ReadHub.Libro.Application.Services
{
    public class PendingLoanService
    {
        private readonly IPendingLoanRepository _pendingLoanRepo;
        private readonly IBookRepository _bookRepo;
        private readonly ILoanRepository _loanRepo;

        public PendingLoanService(IPendingLoanRepository pendingLoanRepo, IBookRepository bookRepo, ILoanRepository loanRepo)
        {
            _pendingLoanRepo = pendingLoanRepo;
            _bookRepo = bookRepo;
            _loanRepo = loanRepo;
        }


        //Usuario crea la solicitud de prestamo de un libro
        public async Task CreatePendingLoanAsync(CreatePendingLoanDto dto)
        {
            foreach (var book in dto.Books)
            {
                var loanDate = DateTime.UtcNow;
                var returnDate = dto.ReturnDate ?? loanDate.AddDays(7);

                var pendingLoan = new PendingLoanBook
                {
                    Id = Guid.NewGuid(),
                    UserId = dto.UserId,
                    BookId = book.BookId,
                    RequestedQuantity = book.RequestedQuantity,
                    LoanDate = loanDate,
                    ReturnDate = returnDate
                };

                await _pendingLoanRepo.AddAsync(pendingLoan);
            }

            await _pendingLoanRepo.SaveChangesAsync();
        }


        // Bibliotecario aprueba la solicitud del prestamo del usuario 
        public async Task ApprovePendingLoanAsync(Guid id, int approvedQuantity)
        {
            var pending = await _pendingLoanRepo.GetByIdAsync(id);
            if (pending == null)
                throw new KeyNotFoundException("Solicitud no encontrada.");

            if (pending.IsApproved)
                throw new InvalidOperationException("Esta solicitud ya fue aprobada.");

            if (pending.IsRejected)
                throw new InvalidOperationException("Esta solicitud ya fue rechazada.");

            var book = await _bookRepo.GetByIdAsync(pending.BookId);
            if (book == null)
                throw new KeyNotFoundException("Libro no encontrado.");

            if (book.Stock < approvedQuantity)
                throw new InvalidOperationException("Stock insuficiente.");

            pending.IsApproved = true;
            pending.IsRejected = false;
            pending.ApprovedQuantity = approvedQuantity;
            pending.ApprovedDate = DateTime.UtcNow;

            var loan = new Loan
            {
                Id = Guid.NewGuid(),
                UserId = pending.UserId,
                LoanDate = DateTime.UtcNow,
                ReturnDate = pending.ReturnDate ?? DateTime.UtcNow.AddDays(7),
                Items = new List<LoanItem>
        {
            new LoanItem
            {
                Id = Guid.NewGuid(),
                BookId = pending.BookId,
                Quantity = approvedQuantity
            }
        }
            };


            book.Stock -= approvedQuantity;
            await _bookRepo.UpdateAsync(book);

            await _loanRepo.AddAsync(loan);
            await _loanRepo.SaveChangesAsync();
            await _pendingLoanRepo.SaveChangesAsync();
        }


        // Bibliotecario rechaza la solicitd del prestamo 
        public async Task RejectPendingLoanAsync(Guid id)
        {
            var pending = await _pendingLoanRepo.GetByIdAsync(id);
            if (pending == null)
                throw new KeyNotFoundException("Solicitud no encontrada.");

            if (pending.IsApproved)
                throw new InvalidOperationException("Esta solicitud ya fue aprobada.");

            if (pending.IsRejected)
                throw new InvalidOperationException("Esta solicitud ya fue rechazada.");

            pending.IsRejected = true;
            pending.IsApproved = false;
            pending.ApprovedDate = DateTime.UtcNow;

            await _pendingLoanRepo.SaveChangesAsync();
        }


        // Ver todas las solicitudes
        public async Task<IEnumerable<PendingLoanBook>> GetAllAsync()
        {
            return await _pendingLoanRepo.GetAllAsync();
        }
    }
}
