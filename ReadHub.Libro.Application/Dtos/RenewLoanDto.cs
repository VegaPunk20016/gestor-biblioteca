using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReadHub.Libro.Application.Dtos
{
    public class RenewLoanDto
    {
        public Guid LoanId { get; set; }
        [Range(1, 30, ErrorMessage = "Los días extra deben ser entre 1 y 30.")]
        public int ExtraDays { get; set; } 
    }
}
