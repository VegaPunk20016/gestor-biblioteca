using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ReadHub.Libro.Domain.Interfaces;

namespace ReadHub.Libro.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class MultasController : ControllerBase
    {
        private readonly IFineRepository _fineRepo;

        public MultasController(IFineRepository fineRepo)
        {
            _fineRepo = fineRepo;
        }

        
        // CONSULTAR TODAS LAS MULTAS 
        [HttpGet]
        [Authorize(Roles = "Bibliotecario")]
        public async Task<IActionResult> GetAll()
        {
            var fines = await _fineRepo.GetAllAsync();
            return Ok(fines);
        }


        // CONSULTAR MULTAS DE UN USUARIO

        [HttpGet("usuario/{userId}")]
        [Authorize(Roles = "Usuario,Bibliotecario")]
        public async Task<IActionResult> GetByUser(Guid userId)
        {
            var fines = await _fineRepo.GetFinesByUser(userId);
            return Ok(fines);
        }


        // CONSULTAR MULTAS POR PRÉSTAMO
        [HttpGet("loan/{loanId}")]
        [Authorize(Roles = "Usuario,Bibliotecario")]
        public async Task<IActionResult> GetByLoan(Guid loanId)
        {
            var fines = await _fineRepo.GetFinesByLoan(loanId);
            return Ok(fines);
        }


        // PAGAR MULTA
       
        [HttpPut("pagar/{fineId}")]
        [Authorize(Roles = "Usuario,Bibliotecario")]
        public async Task<IActionResult> PayFine(Guid fineId)
        {
            var fine = await _fineRepo.GetByIdAsync(fineId);

            if (fine == null)
                return NotFound(new { message = "La multa no fue encontrada" });

            if (fine.IsPaid)
                return BadRequest(new { message = "Esta multa ya está pagada" });

            fine.IsPaid = true;
            await _fineRepo.SaveChangesAsync();

            return Ok(new { message = "Multa pagada correctamente" });
        }
    }
}