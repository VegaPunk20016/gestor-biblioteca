using Microsoft.AspNetCore.Identity;
using ReadHub.Domain.Entities;
using ReadHub.Domain.Interfaces;

public class UserRoleService
{
    private readonly IUserRepository _userRepository;
    private readonly IRoleRepository _roleRepository;
    private readonly IUserRoleRepository _userRoleRepository;
    private readonly PasswordHasher<User> _passwordHasher;

    public UserRoleService(
        IUserRepository userRepository,
        IRoleRepository roleRepository,
        IUserRoleRepository userRoleRepository)
    {
        _userRepository = userRepository;
        _roleRepository = roleRepository;
        _userRoleRepository = userRoleRepository;
        _passwordHasher = new PasswordHasher<User>();
    }

    public async Task AssignRoleAsync(string username, string email, string password, string roleName)
    {
        var user = await _userRepository.GetByEmailAsync(email);
        if (user == null || !user.Username.Equals(username, StringComparison.OrdinalIgnoreCase))
            throw new Exception("El usuario no existe o los datos no coinciden.");


        var result = _passwordHasher.VerifyHashedPassword(user, user.PasswordHash, password);
        if (result == PasswordVerificationResult.Failed)
            throw new Exception("Contraseña incorrecta.");

        var role = await _roleRepository.GetByNameAsync(roleName);
        if (role == null)
            throw new Exception($"El rol '{roleName}' no existe. Debes crearlo manualmente.");

        if (user.UserRoles.Any(ur => ur.RoleId == role.Id))
            throw new Exception($"El usuario '{username}' ya tiene el rol '{roleName}'.");

        var userRole = new UserRole
        {
            UserId = user.Id,
            RoleId = role.Id
        };

        await _userRoleRepository.AddAsync(userRole);
        await _userRoleRepository.SaveChangesAsync();
    }
}
