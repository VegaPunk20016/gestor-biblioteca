using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ReadHub.Application.Dtos;
using ReadHub.Application.Services;

namespace ReadHub.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class RolesController : ControllerBase
    {
        private readonly UserRoleService _userRoleService;

        public RolesController(UserRoleService userRoleService)
        {
            _userRoleService = userRoleService;
        }

        [HttpPost("assign")]
        public async Task<IActionResult> AssignRole([FromBody] AssignRoleRequest dto)
        {
            await _userRoleService.AssignRoleAsync(dto.Username, dto.Email, dto.Password, dto.RoleName);
            return Ok($"Rol '{dto.RoleName}' asignado correctamente al usuario '{dto.Username}'.");
        }
    }
}