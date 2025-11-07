using Microsoft.AspNetCore.Identity;
using ReadHub.Application.Common;
using ReadHub.Application.Dtos;
using ReadHub.Application.Interfaces;
using ReadHub.Domain.Entities;
using ReadHub.Domain.Interfaces;

namespace ReadHub.Application.Services
{
    public class AuthService
    {
        private readonly IUserRepository _userRepository;
        private readonly IRoleRepository _roleRepository;
        private readonly IUserRoleRepository _userRoleRepository;
        private readonly IJwtTokenGenerator _jwtGenerator;
        private readonly PasswordHasher<User> _passwordHasher;

        public AuthService(
            IUserRepository userRepository,
            IRoleRepository roleRepository,
            IUserRoleRepository userRoleRepository,
            IJwtTokenGenerator jwtGenerator)
        {
            _userRepository = userRepository;
            _roleRepository = roleRepository;
            _userRoleRepository = userRoleRepository;
            _jwtGenerator = jwtGenerator;
            _passwordHasher = new PasswordHasher<User>();
        }

        public async Task<Result> RegisterAsync(RegisterDto registerdto)
        {
            var existingUser = await _userRepository.GetByEmailAsync(registerdto.Email);
            if (existingUser != null)
                return Result.Fail("El email ya está registrado.");
            var user = new User
            {
                Id = Guid.NewGuid(),
                Username = registerdto.Username,
                Email = registerdto.Email,
                PhoneNumber = registerdto.Phone
            };

            user.PasswordHash = _passwordHasher.HashPassword(user, registerdto.Password);
            await _userRepository.AddAsync(user);
            var userRoleEntity = await _roleRepository.GetByNameAsync("Usuario");
            if (userRoleEntity == null)
                return Result.Fail("El rol 'Usuario' no existe. Contacta al administrador del sistema.");

            var userRole = new UserRole
            {
                UserId = user.Id,
                RoleId = userRoleEntity.Id
            };

            await _userRoleRepository.AddAsync(userRole);

            await _userRepository.SaveChangesAsync();
            await _userRoleRepository.SaveChangesAsync();

            return Result.Ok(null, "Usuario registrado correctamente");
        }

        public async Task<Result> LoginAsync(LoginDto logindto)
        {
            var user = await _userRepository.GetByEmailAsync(logindto.Email);
            if (user == null)
                return Result.Fail("Usuario no encontrado.");
            var verification = _passwordHasher.VerifyHashedPassword(user, user.PasswordHash, logindto.Password);
            if (verification == PasswordVerificationResult.Failed)
                return Result.Fail("Contraseña incorrecta.");
            var token = _jwtGenerator.GenerateToken(user);

            return Result.Ok(token, "Inicio de sesión exitoso.");
        }
    }
}
