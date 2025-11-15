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

        // ============================================================
        // 1️⃣ CONSULTAR TODAS LAS MULTAS (solo bibliotecario)
        // ============================================================
        [HttpGet]
        [Authorize(Roles = "Bibliotecario")]
        public async Task<IActionResult> GetAll()
        {
            var fines = await _fineRepo.GetAllAsync();
            return Ok(fines);
        }


        // ============================================================
        // 2️⃣ CONSULTAR MULTAS DE UN USUARIO
        // ============================================================
        [HttpGet("usuario/{userId}")]
        [Authorize(Roles = "Usuario,Bibliotecario")]
        public async Task<IActionResult> GetByUser(Guid userId)
        {
            var fines = await _fineRepo.GetFinesByUser(userId);
            return Ok(fines);
        }


        // ============================================================
        // 3️⃣ CONSULTAR MULTAS POR PRÉSTAMO
        // ============================================================
        [HttpGet("loan/{loanId}")]
        [Authorize(Roles = "Usuario,Bibliotecario")]
        public async Task<IActionResult> GetByLoan(Guid loanId)
        {
            var fines = await _fineRepo.GetFinesByLoan(loanId);
            return Ok(fines);
        }


        // ============================================================
        // 4️⃣ PAGAR MULTA
        // ============================================================
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