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
        private readonly IEmailService _emailService;
        private readonly PasswordHasher<User> _passwordHasher;

        public AuthService(
            IUserRepository userRepository,
            IRoleRepository roleRepository,
            IUserRoleRepository userRoleRepository,
            IJwtTokenGenerator jwtGenerator,
            IEmailService emailService)
        {
            _userRepository = userRepository;
            _roleRepository = roleRepository;
            _userRoleRepository = userRoleRepository;
            _jwtGenerator = jwtGenerator;
            _emailService = emailService;
            _passwordHasher = new PasswordHasher<User>();
        }


        //Registrar usuario
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

            //  Enviar correo de bienvenida
            await _emailService.SendEmailAsync(
    user.Email,
    "Bienvenido a ReadHub",
    $@"
<table width='100%' cellpadding='0' cellspacing='0' style='font-family: Arial, sans-serif; background-color:#f4f4f4; padding:25px;'>
    <tr>
        <td align='center'>
            <table width='600' cellpadding='0' cellspacing='0' style='background:#ffffff; border-radius:10px; overflow:hidden;'>

                <!-- Header -->
                <tr>
                    <td align='center' style='background:#6e3c11; padding:20px;'>
                        <h2 style='color:white; margin:0;'>Bienvenido a ReadHub</h2>
                    </td>
                </tr>

                <!-- Mensaje principal -->
                <tr>
                    <td style='padding:25px 30px; color:#444; font-size:16px; line-height:1.6;'>
                        <p>Hola <b>{user.Username}</b>,</p>
                        <p>Tu cuenta ha sido creada exitosamente en <b>ReadHub</b>.</p>

                        <p>Puedes iniciar sesión con tu correo:</p>

                        <p style='font-size:18px; font-weight:bold; color:#1a73e8;'>{user.Email}</p>

                        <p>Si necesitas ayuda, estamos aquí para apoyarte.</p>
                    </td>
                </tr>

                <!-- Footer -->
                <tr>
                    <td align='center' style='background:#6e3c11; color:white; padding:15px; font-size:14px;'>
                        © {DateTime.UtcNow.Year} ReadHub
                    </td>
                </tr>

            </table>
        </td>
    </tr>
</table>
"
);


            return Result.Ok(null, "Usuario registrado correctamente");
        }


        //Iniciar sesión
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
