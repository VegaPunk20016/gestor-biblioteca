using System.ComponentModel.DataAnnotations;

namespace ReadHub.Domain.Entities
{
    public class UserRole
    {
        public Guid UserId { get; set; }
        [Required]
        public User User { get; set; }
        public Guid RoleId { get; set; }
        [Required]
        public Role Role { get; set; }
    }
}
