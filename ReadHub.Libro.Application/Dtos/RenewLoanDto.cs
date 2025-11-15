using System.ComponentModel.DataAnnotations;

namespace ReadHub.Libro.Application.Dtos
{
    public class RenewLoanDto
    {
        public Guid LoanId { get; set; }
        [Range(1, 30, ErrorMessage = "Los días extra deben ser entre 1 y 30.")]
        public int ExtraDays { get; set; } 
    }
}
