using ReadHub.Domain.Entities;
using ReadHub.Domain.Interfaces;
using System.Security.Cryptography;
using System.Text;

public class UserRoleService
{
    private readonly IUserRepository _userRepository;
    private readonly IRoleRepository _roleRepository;
    private readonly IUserRoleRepository _userRoleRepository;

    public UserRoleService(
        IUserRepository userRepository,
        IRoleRepository roleRepository,
        IUserRoleRepository userRoleRepository)
    {
        _userRepository = userRepository;
        _roleRepository = roleRepository;
        _userRoleRepository = userRoleRepository;
    }

    public async Task AssignRoleAsync(string username, string email, string password, string roleName)
    {
        // 🔹 Buscar usuario por email
        var user = await _userRepository.GetByEmailAsync(email);
        if (user == null || !user.Username.Equals(username, StringComparison.OrdinalIgnoreCase))
            throw new Exception("El usuario no existe o los datos no coinciden.");

        // 🔹 Validar contraseña (igual que en AuthService)
        using var sha256 = SHA256.Create();
        var passwordHash = Convert.ToBase64String(sha256.ComputeHash(Encoding.UTF8.GetBytes(password)));
        if (user.PasswordHash != passwordHash)
            throw new Exception("Contraseña incorrecta.");

        // 🔹 Buscar el rol por nombre
        var role = await _roleRepository.GetByNameAsync(roleName);
        if (role == null)
            throw new Exception($"El rol '{roleName}' no existe. Debes crearlo manualmente.");

        // 🔹 Verificar si ya tiene ese rol
        if (user.UserRoles.Any(ur => ur.RoleId == role.Id))
            throw new Exception($"El usuario '{username}' ya tiene el rol '{roleName}'.");

        // 🔹 Crear la relación
        var userRole = new UserRole
        {
            UserId = user.Id,
            RoleId = role.Id
        };

        await _userRoleRepository.AddAsync(userRole);
        await _userRoleRepository.SaveChangesAsync();
    }
}
