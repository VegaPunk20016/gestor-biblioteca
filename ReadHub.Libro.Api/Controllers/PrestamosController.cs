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
        [HttpPost]
        [Authorize(Roles = "Bibliotecario")]
        public async Task<IActionResult> CreateLoan([FromBody] CreateLoanDto dto)
        {
            var loan = await _loanService.CreateLoanAsync(dto);
            return Ok(loan);
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
        [HttpPut("Renovacion")]
        [Authorize(Roles = "Usuario,Bibliotecario")]
        public async Task<IActionResult> RenewLoan([FromBody] RenewLoanDto dto)
        {
            await _loanService.RenewLoanAsync(dto.LoanId, dto.ExtraDays);
            return Ok(new { message = "Préstamo renovado exitosamente." });
        }


    }
}
