using ReadHub.Libro.Application.Dtos;
using ReadHub.Libro.Domain.Entities;

namespace ReadHub.Libro.Application.Interfaces
{
    public interface ILoanRenewalService
    {
        Task<LoanRenewalRequest> CreateRequestAsync(CreateLoanRenewalRequestDto dto);
        Task<IEnumerable<LoanRenewalRequest>> GetPendingAsync();
        Task<bool> ApproveAsync(Guid requestId);
        Task<bool> RejectAsync(Guid requestId, string? notes = null);
    }
}
