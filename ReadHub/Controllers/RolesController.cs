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

        [HttpPut("assign")]
        public async Task<IActionResult> AssignRole([FromBody] AssignRoleRequest dto)
        {
            try
            {
                var result = await _userRoleService.AssignRoleAsync(dto);

                return result.Success ? Ok(result) : BadRequest(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }

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
