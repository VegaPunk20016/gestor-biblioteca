using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ReadHub.Libro.Application.Dtos;
using ReadHub.Libro.Application.Services;

namespace ReadHub.Libro.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SolicitudPrestamosController : ControllerBase
    {
        private readonly PendingLoanService _service;

        public SolicitudPrestamosController(PendingLoanService service)
        {
            _service = service;
        }

        // Usuario crea solicitud
        [HttpPost("usuarios")]
        [Authorize(Roles = "Usuario")]
        public async Task<IActionResult> CreatePendingLoan([FromBody] CreatePendingLoanDto dto)
        {
            await _service.CreatePendingLoanAsync(dto);
            return Ok(new { message = "Solicitud enviada correctamente" });
        }

        // Bibliotecario aprueba
        [HttpPut("Aprobar")]
        [Authorize(Roles = "Bibliotecario")]
        public async Task<IActionResult> Approve([FromBody] ApprovePendingLoanDto dto)
        {
            try
            {
                await _service.ApprovePendingLoanAsync(dto.PendingLoanId, dto.ApprovedQuantity);
                return Ok(new { message = "Solicitud aprobada" });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }



        // Bibliotecario rechaza
        [HttpPut("Rechazar/{id}")]
        [Authorize(Roles = "Bibliotecario")]
        public async Task<IActionResult> Reject(Guid id)
        {
            try
            {
                await _service.RejectPendingLoanAsync(id);
                return Ok(new { message = "Solicitud rechazada correctamente." });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }


        // Ver todas las solicitudes
        [HttpGet]
        [Authorize(Roles = "Bibliotecario")]
        public async Task<IActionResult> GetAll()
        {
            var result = await _service.GetAllAsync();
            return Ok(result);
        }
    }
}
