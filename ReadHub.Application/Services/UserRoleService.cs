using Microsoft.AspNetCore.Identity;
using ReadHub.Application.Common;
using ReadHub.Application.Dtos;
using ReadHub.Domain.Entities;
using ReadHub.Domain.Interfaces;

namespace ReadHub.Application.Services
{
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

        //AIGNAR ROL A USUARIO
        public async Task<Result> AssignRoleAsync(AssignRoleRequest assignRoleRequestdto)
        {
            var user = await _userRepository.GetByEmailAsync(assignRoleRequestdto.Email);

            if (user == null || !user.Username.Equals(assignRoleRequestdto.Username, StringComparison.OrdinalIgnoreCase))
                return Result.Fail("El usuario no existe o los datos no coinciden.");

            var result = _passwordHasher.VerifyHashedPassword(user, user.PasswordHash, assignRoleRequestdto.Password);
            if (result == PasswordVerificationResult.Failed)
                return Result.Fail("Contraseña incorrecta.");

            var role = await _roleRepository.GetByNameAsync(assignRoleRequestdto.RoleName);
            if (role == null)
                return Result.Fail($"El rol '{assignRoleRequestdto.RoleName}' no existe.");

            if (user.UserRoles.Any())
            {
                await _userRoleRepository.RemoveRangeAsync(user.UserRoles);
            }

            var userRole = new UserRole
            {
                UserId = user.Id,
                RoleId = role.Id
            };

            await _userRoleRepository.AddAsync(userRole);
            await _userRoleRepository.SaveChangesAsync();

            return Result.Ok(null, $"Rol cambiado correctamente a '{assignRoleRequestdto.RoleName}'.");
        }


        //OBTENER USUARIOS POR ROL "USUARIO"
        public async Task<Result> GetUsersByRoleUsuarioAsync()
        {
            var users = await _userRepository.GetUsersByRoleAsync("Usuario");

            var result = users.Select(u => new
            {
                u.Id,
                u.Username,
                u.Email,
                u.PhoneNumber
            });

            return Result.Ok(result);
        }

    }
}
