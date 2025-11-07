using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReadHub.Libro.Domain.Entities
{
    public class Loan
    {
        public Guid Id { get; set; }

        [Required]
        public Guid UserId { get; set; } // Usuario del microservicio de Auth

        public DateTime LoanDate { get; set; } = DateTime.UtcNow;
        public DateTime ReturnDate { get; set; } // Fecha límite

        // 👇 Relación uno a muchos
        public List<LoanItem> Items { get; set; } = new();
    }

}
