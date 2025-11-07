using System.ComponentModel.DataAnnotations;

namespace ReadHub.Domain.Entities
{
    public class User
    {
        public Guid Id { get; set; }
        [Required]
        public string Username { get; set; }
        [Required]
        public string Email { get; set; }
        [Required]
        public string PasswordHash { get; set; }
        [Required]
        public string PhoneNumber { get; set; }
        public ICollection<UserRole> UserRoles { get; set; } = new List<UserRole>();
    }
}
