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
        public async Task<IActionResult> AssignRole([FromBody] AssignRoleRequest assignRoleRequestdto)
        {
            await _userRoleService.AssignRoleAsync(assignRoleRequestdto.Username, assignRoleRequestdto.Email,
                assignRoleRequestdto.Password, assignRoleRequestdto.RoleName);
            return Ok($"Rol '{assignRoleRequestdto.RoleName}' asignado correctamente al usuario '{assignRoleRequestdto.Username}'.");
        }
    }
}