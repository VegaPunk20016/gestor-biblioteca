using System.ComponentModel.DataAnnotations;

namespace ReadHub.Domain.Entities
{
    public class Role
    {
        public Guid Id { get; set; }

        [Required]
        public string Name { get; set; }
        public string? Description { get; set; }

        public ICollection<UserRole> UserRoles { get; set; } = new List<UserRole>();
    }

}
