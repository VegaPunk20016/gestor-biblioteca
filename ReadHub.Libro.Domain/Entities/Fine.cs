namespace ReadHub.Libro.Domain.Entities
{
    public class Fine
    {
        public Guid Id { get; set; } = Guid.NewGuid();

        public Guid LoanId { get; set; }
        public Loan Loan { get; set; }

        public Guid UserId { get; set; }

        public int DaysLate { get; set; }

        public int WeekNumber { get; set; } 

        public decimal Amount { get; set; }  

        public bool IsPaid { get; set; } = false;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }

}
