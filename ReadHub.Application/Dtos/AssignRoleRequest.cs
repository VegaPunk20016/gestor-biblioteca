namespace ReadHub.Application.Dtos
{
    public class AssignRoleRequest
    {
        public string Username { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string RoleName { get; set; }
    }
}
