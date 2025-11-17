using System.ComponentModel.DataAnnotations;

namespace ReadHub.Libro.Domain.Entities
{
    public class LoanRenewalRequest
    {
        public Guid Id { get; set; } = Guid.NewGuid();

        [Required]
        public Guid LoanId { get; set; }

        [Required]
        public Guid UserId { get; set; }

        public int RequestedExtraDays { get; set; }

        public bool IsApproved { get; set; } = false;
        public bool IsRejected { get; set; } = false;

        public DateTime RequestedAt { get; set; } = DateTime.UtcNow;
        public DateTime? ApprovedAt { get; set; }
   
    }
}
