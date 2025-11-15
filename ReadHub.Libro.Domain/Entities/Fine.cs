using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReadHub.Libro.Domain.Entities
{
    public class Fine
    {
        public Guid Id { get; set; } = Guid.NewGuid();

        public Guid LoanId { get; set; }
        public Loan Loan { get; set; }

        public Guid UserId { get; set; }

        public int DaysLate { get; set; }

        public int WeekNumber { get; set; }  // 1 = semana 1, 2 = semana 2…

        public decimal Amount { get; set; }  // monto de la multa

        public bool IsPaid { get; set; } = false;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }

}
