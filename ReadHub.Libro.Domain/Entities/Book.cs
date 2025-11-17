using System.ComponentModel.DataAnnotations;

namespace ReadHub.Libro.Domain.Entities
{
    public class Book
    {
        public Guid Id { get; set; }

        [Required]
        public string Title { get; set; }

        [Required]
        public string Author { get; set; }

        public string? Editorial { get; set; }

        public int Year { get; set; }

        [Required]
        public string Category { get; set; }

        [Required]
        public string ISBN { get; set; }
        [Required]
        public int Stock { get; set; }
    }
}
