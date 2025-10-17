using Microsoft.AspNetCore.Mvc;
using ReadHub.Application.Dtos;
using ReadHub.Application.Services;

namespace ReadHub.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly AuthService _authService;

        public AuthController(AuthService authService)
        {
            _authService = authService;
        }

        // Registro de usuario
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterDto registerDto)
        {
            try
            {
                await _authService.RegisterAsync(
                    registerDto.Username,
                    registerDto.Email,
                    registerDto.Phone,
                    registerDto.Password
                );

                return Ok(new { message = "Usuario registrado correctamente" });
            }
            catch (Exception ex)
            {
                // Devuelve error si el email ya está registrado o cualquier otro fallo
                return BadRequest(new { error = ex.Message });
            }
        }

        // Login de usuario
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto loginDto)
        {
            var token = await _authService.LoginAsync(loginDto.Email, loginDto.Password);
            if (token == null)
                return Unauthorized(new { message = "Credenciales inválidas" });

            return Ok(new { message = "Inicio de sesión exitoso", token });
        }
    }
}
