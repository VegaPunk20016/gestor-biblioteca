using Microsoft.AspNetCore.Mvc;
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

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterDto dto)
        {
           
            return Ok(new { message = "Usuario registrado correctamente"});
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto dto)
        {
            var token = await _authService.LoginAsync(dto.Email, dto.Password);
            if (token == null)
                return Unauthorized("Credenciales inválidas");

            return Ok(new { message = "Inicio de sesión exitoso", token });
        }
    }

    public record RegisterDto(string Username, string Email, string Phone, string Password);
    public record LoginDto(string Email, string Password);
}