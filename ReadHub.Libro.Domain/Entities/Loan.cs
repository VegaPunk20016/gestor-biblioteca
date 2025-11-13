using System.ComponentModel.DataAnnotations;

namespace ReadHub.Libro.Domain.Entities
{
    public class Loan
    {
        public Guid Id { get; set; }
        [Required]
        public Guid UserId { get; set; }
        public DateTime LoanDate { get; set; } = DateTime.UtcNow;
        public DateTime ReturnDate { get; set; }
        public List<LoanItem> Items { get; set; } = new();
    }
}
