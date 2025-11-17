using System.ComponentModel.DataAnnotations;

namespace ReadHub.Libro.Application.Dtos
{
    public class CreatePendingLoanDto
    {
        [Required]
        public Guid UserId { get; set; }
        public DateTime? ReturnDate { get; set; }

        [Required]
        public List<PendingLoanBookItemDto> Books { get; set; } = new();
    }

    public class PendingLoanBookItemDto
    {
        [Required]
        public Guid BookId { get; set; }

        [Required]
        public int RequestedQuantity { get; set; }
    }

    public class ApprovePendingLoanDto
    {
        [Required]
        public Guid PendingLoanId { get; set; }

        [Required]
        public int ApprovedQuantity { get; set; }
    }
}
