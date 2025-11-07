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

}
