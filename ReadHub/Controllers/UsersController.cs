using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ReadHub.Application.Services;
using ReadHub.Domain.Interfaces;

namespace ReadHub.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UsersController : Controller
    {
        private readonly IUserRepository _userRepo;
        private readonly UserRoleService _userRoleService;
        public UsersController(IUserRepository userRepo, UserRoleService userRoleService)
        {
            _userRepo = userRepo;
            _userRoleService = userRoleService;
        }

        // Usuario por id
        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var user = await _userRepo.GetByIdAsync(id);
            if (user == null)
                return NotFound();

            return Ok(new
            {
                user.Id,
                user.Username,
                user.Email,
                user.PhoneNumber
            });
        }

        //Todos los usuarios con rol "Usuario"
        [HttpGet("usuarios")]
        [Authorize(Roles = "Bibliotecario")]
        public async Task<IActionResult> GetUsuarios()
        {
            try
            {
                var result = await _userRoleService.GetUsersByRoleUsuarioAsync();
                return result.Success ? Ok(result.Data) : BadRequest(result.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }
    }
}
