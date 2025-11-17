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


        // Usuario crea una solicitud de prestamo de un libro 
        [HttpPost]
        public async Task<IActionResult> CreatePendingLoan(CreatePendingLoanDto dto)
        {
            try
            {
                await _service.CreatePendingLoanAsync(dto);
                return Ok("Solicitud creada correctamente.");
            }
            catch (InvalidOperationException ex)
            {
              
                return BadRequest(ex.Message);
            }
            catch (KeyNotFoundException ex)
            {
                
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                
                return StatusCode(500, "Error interno del servidor: " + ex.Message);
            }
        }


        // Bibliotecario aprueba la solicitud de prestamo del usuario 
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



        // Bibliotecario rechaza la solicitd del prestamo 
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
