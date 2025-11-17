using System.ComponentModel.DataAnnotations;

namespace ReadHub.Libro.Application.Dtos
{
    public class CreateLoanDto
    {
        [Required]
        public Guid UserId { get; set; }

        public int LoanDays { get; set; } = 7;

        [Required]
        public List<LoanBookItemDto> Books { get; set; } = new();
    }

    public class LoanBookItemDto
    {
        public Guid BookId { get; set; }
        public int Quantity { get; set; } = 1;
    }
    public class ReturnLoanItemRequest
    {
        public int Quantity { get; set; } = 0;
    }

    public class OverdueLoanDto
    {
        public Guid LoanId { get; set; }
        public Guid UserId { get; set; }
        public DateTime LoanDate { get; set; }
        public DateTime ReturnDate { get; set; }
        public List<OverdueLoanItemDto> Items { get; set; } = new();
    }

    public class OverdueLoanItemDto
    {
        public string BookTitle { get; set; } = string.Empty;
        public int Quantity { get; set; }
        public bool IsReturned { get; set; }
    }


}
