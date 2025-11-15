using System.ComponentModel.DataAnnotations;

namespace ReadHub.Libro.Application.Dtos
{
    
    public class BookDto
    {
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
