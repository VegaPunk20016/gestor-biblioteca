using ReadHub.Libro.Domain.Entities;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

public class LoanItem
{
    public Guid Id { get; set; }

    [Required]
    public Guid LoanId { get; set; }

    [JsonIgnore]
    public Loan Loan { get; set; }

    [Required]
    public Guid BookId { get; set; }
    public Book Book { get; set; }

    public int Quantity { get; set; } = 1;

    public DateTime? ReturnedAt { get; set; }

    public bool IsReturned => ReturnedAt.HasValue;
}
