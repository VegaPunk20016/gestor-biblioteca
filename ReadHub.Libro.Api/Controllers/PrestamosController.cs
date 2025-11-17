using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ReadHub.Libro.Application.Dtos;
using ReadHub.Libro.Application.Services;

namespace ReadHub.Libro.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PrestamosController : ControllerBase
    {
        private readonly LoanService _loanService;

        public PrestamosController(LoanService loanService)
        {
            _loanService = loanService;
        }

        // Prestar libro
        [HttpPost("PrestarBibliotecario")]
        [Authorize(Roles = "Bibliotecario")]
        public async Task<IActionResult> CreateLoan(CreateLoanDto dto)
        {
            try
            {
                await _loanService.CreateLoanAsync(dto);
                return Ok("Préstamo creado correctamente.");
            }
            catch (InvalidOperationException ex)
            {
                
                return BadRequest(ex.Message);
            }
            catch (Exception)
            {
              
                return StatusCode(500, "Ocurrió un error interno.");
            }
        }

        // Devolver libro
        [HttpPut("return-item/{loanItemId}")]
        [Authorize(Roles = "Bibliotecario")]

        public async Task<IActionResult> ReturnLoanItem(Guid loanItemId, [FromBody] ReturnLoanItemRequest request)
        {
            var result = await _loanService.ReturnLoanItemAsync(loanItemId, request.Quantity);
            if (result == null) return NotFound(new { message = "Loan item no encontrado o ya devuelto" });
            return Ok(result);
        }


        //Renovar un libro 
        [HttpPut("RenovacionBibliotecario")]
        [Authorize(Roles = "Bibliotecario")]

        public async Task<IActionResult> RenewLoan([FromBody] RenewLoanDto dto)
        {
            await _loanService.RenewLoanAsync(dto.LoanId, dto.ExtraDays);
            return Ok(new { message = "Préstamo renovado exitosamente." });
        }

        //Prestamos vencidos
        [HttpGet("Prestamosvencidos")]
        [Authorize(Roles = "Bibliotecario")]
        public async Task<IActionResult> GetOverdueLoans()
        {
            try
            {
                var overdueLoans = await _loanService.GetOverdueLoansAsync();

                if (!overdueLoans.Any())
                    return Ok("No hay préstamos vencidos.");

                return Ok(overdueLoans);
            }
            catch (Exception ex)
            {
         
                return BadRequest(ex.Message);
            }
        }


        //Prestamos vencidos por usuario 
        [HttpGet("Usuario/{userId}/Vencidos")]
        [Authorize(Roles = "Usuario,Bibliotecario")]
        public async Task<IActionResult> GetUserOverdueLoans(Guid userId)
        {
            try
            {
                var loans = await _loanService.GetUserOverdueLoansAsync(userId);

                if (!loans.Any())
                    return Ok("El usuario no tiene préstamos vencidos.");

                return Ok(loans);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }


    }
}
