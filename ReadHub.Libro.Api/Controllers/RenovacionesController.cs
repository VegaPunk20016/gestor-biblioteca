using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ReadHub.Libro.Application.Dtos;
using ReadHub.Libro.Application.Services;
using System.Security.Claims;

namespace ReadHub.Libro.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class RenovacionesController : Controller
    {

        private readonly LoanRenewalService _loanRenewalService;
        private readonly LoanService _loanService;


        public RenovacionesController(LoanRenewalService loanRenewalService, LoanService loanService)
            {
            _loanRenewalService = loanRenewalService;
            _loanService = loanService;
            }


        //Usuario puede solicitar renovación
        [HttpPost("solicitar")]
        [Authorize(Roles = "Usuario")]
        public async Task<IActionResult> Create([FromBody] CreateRenewalRequestDto dto)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);

            if (userIdClaim == null)
                return Unauthorized(new { message = "No se pudo obtener el usuario desde el token." });

            var userId = Guid.Parse(userIdClaim.Value);

            await _loanRenewalService.CreateRenewalRequestAsync(userId, dto);

            return Ok(new { message = "Solicitud enviada correctamente" });
        }



        // Bibliotecario aprueba
        [HttpPut("aprobar/{id}")]
            [Authorize(Roles = "Bibliotecario")]
            public async Task<IActionResult> Approve(Guid id)
            {
                await _loanRenewalService.ApproveRenewalAsync(id);
                return Ok(new { message = "Renovación aprobada" });
            }

            // Bibliotecario rechaza
            [HttpPut("rechazar/{id}")]
            [Authorize(Roles = "Bibliotecario")]
            public async Task<IActionResult> Reject(Guid id)
            {
                await _loanRenewalService.RejectRenewalAsync(id);
            return Ok(new { message = "Renovación rechazada" });

            }

            //Todas las renovaciones pendientes por aceptar o rechazar 
            [HttpGet("RenovPendientes")]
            [Authorize(Roles = "Bibliotecario")]
            public async Task<IActionResult> GetPending()
            {
                var requests = await _loanRenewalService.GetPendingAsync();
                return Ok(requests);
            }

    }
}
