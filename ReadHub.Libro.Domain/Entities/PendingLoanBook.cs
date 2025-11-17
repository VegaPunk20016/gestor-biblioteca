using System.ComponentModel.DataAnnotations;

namespace ReadHub.Libro.Domain.Entities
{
    public class PendingLoanBook
    {
        public Guid Id { get; set; }

        [Required]
        public Guid UserId { get; set; }

        [Required]
        public Guid BookId { get; set; }

        public int RequestedQuantity { get; set; }

        public int? ApprovedQuantity { get; set; }

        public bool IsApproved { get; set; } = false;

        public bool IsRejected { get; set; } = false;
        public DateTime LoanDate { get; set; } = DateTime.UtcNow;
        public DateTime? ReturnDate { get; set; }
        public DateTime? ApprovedDate { get; set; }

    }
}
