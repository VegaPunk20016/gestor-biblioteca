using ReadHub.Libro.Application.Dtos;
using ReadHub.Libro.Application.Interfaces;
using ReadHub.Libro.Domain.Entities;
using ReadHub.Libro.Domain.Interfaces;

namespace ReadHub.Libro.Application.Services
{
    public class LoanRenewalService
    {
        private readonly ILoanRenewalRequestRepository _renewalRepo;
        private readonly ILoanRepository _loanRepo;
        private readonly IFineRepository _fineRepo;
        private readonly IUserService _userService;
        private readonly EmailService _emailService;

        public LoanRenewalService(
            ILoanRenewalRequestRepository renewalRepo,
            ILoanRepository loanRepo,
            IFineRepository fineRepo,
            IUserService userService,
            EmailService emailService)
        {
            _renewalRepo = renewalRepo;
            _loanRepo = loanRepo;
            _fineRepo = fineRepo;
            _userService = userService;
            _emailService = emailService;
        }


        // 1) Usuario solicita renovación

        public async Task CreateRenewalRequestAsync(Guid userId, CreateRenewalRequestDto dto)
        {
            var loan = await _loanRepo.GetByIdAsync(dto.LoanId);
            if (loan == null)
                throw new Exception("El préstamo no existe.");

            if (loan.UserId != userId)
                throw new Exception("No puedes renovar un préstamo que no es tuyo.");

            // No permitir si tiene multas
            var fines = await _fineRepo.GetFinesByUser(userId);
            if (fines.Any(f => f.LoanId == dto.LoanId && !f.IsPaid))
                throw new Exception("No se puede solicitar renovación porque tienes multas pendientes.");

            // No permitir más de una solicitud pendiente
            var existingRequests = await _renewalRepo.GetByLoanIdAsync(dto.LoanId);
            if (existingRequests.Any(r => !r.IsApproved && !r.IsRejected))
                throw new Exception("Ya tienes una solicitud de renovación pendiente.");

            // Crear solicitud
            var request = new LoanRenewalRequest
            {
                LoanId = dto.LoanId,
                UserId = userId,
                RequestedExtraDays = dto.ExtraDays
            };

            await _renewalRepo.AddAsync(request);
            await _renewalRepo.SaveChangesAsync();
        }


        // 2) Bibliotecario aprueba

        public async Task ApproveRenewalAsync(Guid requestId)
        {
            var request = await _renewalRepo.GetByIdAsync(requestId);
            if (request == null)
                throw new Exception("La solicitud no existe.");

            if (request.IsApproved)
                throw new Exception("La solicitud ya fue aprobada.");

            if (request.IsRejected)
                throw new Exception("La solicitud ya fue rechazada, no se puede aprobar.");

            var loan = await _loanRepo.GetByIdAsync(request.LoanId);
            if (loan == null)
                throw new Exception("El préstamo no existe.");

            loan.ReturnDate = loan.ReturnDate.AddDays(request.RequestedExtraDays);

            request.IsApproved = true;
            request.ApprovedAt = DateTime.UtcNow;

            await _loanRepo.SaveChangesAsync();
            await _renewalRepo.SaveChangesAsync();

            // Enviar correo
            string email = await _userService.GetUserEmailAsync(request.UserId);

            string body = $@"
<!DOCTYPE html>
<html>
<head>
<meta charset='UTF-8'>
<style>
    body {{
        background-color: #f4f7fc;
        font-family: 'Segoe UI', Arial, sans-serif;
    }}
    .container {{
        max-width: 600px;
        margin: 40px auto;
        background: #ffffff;
        border-radius: 12px;
        padding: 35px;
        box-shadow: 0 4px 20px rgba(0,0,0,0.1);
    }}
    .header {{
        text-align: center;
        margin-bottom: 25px;
    }}
    .header h1 {{
        background: #6e3c11;
        color: white;
        padding: 15px 0;
        border-radius: 8px;
    }}
    p {{
        color: #333;
        font-size: 16px;
        line-height: 1.6;
    }}
    .date-box {{
        background: #f7e8e8;
        border-left: 4px solid #b88b5c;
        padding: 12px;
        margin: 20px 0;
        border-radius: 5px;
        font-size: 18px;
        font-weight: bold;
        color: #7a5a38;
        text-align: center;
    }}
    .footer {{
        margin-top: 30px;
        text-align: center;
        color: #777;
    }}
</style>
</head>
<body>
<div class='container'>
    <div class='header'>
        <h1>¡Renovación Aprobada!</h1>
    </div>

    <p>Tu solicitud de renovación ha sido aprobada exitosamente.</p>

    <div class='date-box'>
        Nueva fecha de devolución: {loan.ReturnDate:dd/MM/yyyy}
    </div>

    <p>Gracias por utilizar ReadHub.</p>

    <div class='footer'>
        © {DateTime.UtcNow.Year} ReadHub 
    </div>
</div>
</body>
</html>";

            await _emailService.SendEmailAsync(email, "ReadHub: Renovación aprobada", body);
        }

        // 3) Bibliotecario rechaza

        public async Task RejectRenewalAsync(Guid requestId)
        {
            var request = await _renewalRepo.GetByIdAsync(requestId);
            if (request == null)
                throw new Exception("La solicitud no existe.");

            if (request.IsRejected)
                throw new Exception("La solicitud ya fue rechazada.");

            if (request.IsApproved)
                throw new Exception("La solicitud ya fue aprobada, no se puede rechazar.");

            request.IsRejected = true;
            await _renewalRepo.SaveChangesAsync();

            // Enviar correo
            string email = await _userService.GetUserEmailAsync(request.UserId);

            string body = $@"
<!DOCTYPE html>
<html>
<head>
<meta charset='UTF-8'>
<style>
    body {{
        background-color: #f4f7fc;
        font-family: 'Segoe UI', Arial, sans-serif;
    }}
    .container {{
        max-width: 600px;
        margin: 40px auto;
        background: #ffffff;
        border-radius: 12px;
        padding: 35px;
        box-shadow: 0 4px 20px rgba(0,0,0,0.1);
    }}
    .header {{
        text-align: center;
        margin-bottom: 25px;
    }}
    .header h1 {{
        background: #6e3c11;
        color: white;
        padding: 15px 0;
        border-radius: 8px;
    }}
    p {{
        color: #333;
        font-size: 16px;
        line-height: 1.6;
    }}
    .date-box {{
        background: #f7e8e8;
        border-left: 4px solid #b88b5c;
        padding: 12px;
        margin: 20px 0;
        border-radius: 5px;
        font-size: 18px;
        font-weight: bold;
        color: #7a5a38;
        text-align: center;
    }}
    .footer {{
        margin-top: 30px;
        text-align: center;
        color: #777;
    }}
</style>
</head>
<body>
<div class='container'>
    <div class='header'>
        <h1>Renovación Rechazada</h1>
    </div>

    <p>Lamentamos informarte que tu solicitud de renovación ha sido rechazada.</p>

    <p>Si necesitas más información, puedes comunicarte con la biblioteca.</p>

    <div class='footer'>
        © {DateTime.UtcNow.Year} ReadHub 
    </div>
</div>
</body>
</html>";

            await _emailService.SendEmailAsync(email, "ReadHub: Renovación rechazada", body);
        }


        // Obtener todas las solicitudes pendientes
        public async Task<IEnumerable<LoanRenewalRequest>> GetPendingAsync()
        {
            return await _renewalRepo.GetPendingAsync();
        }
    }
}
