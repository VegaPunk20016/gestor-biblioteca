using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using ReadHub.Libro.Domain.Entities;
using ReadHub.Libro.Domain.Interfaces;
using System.Net.Http.Json;

namespace ReadHub.Libro.Application.Services
{
    public class LoanExpirationChecker : BackgroundService
    {
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly IConfiguration _config;

        public LoanExpirationChecker(IServiceScopeFactory scopeFactory, IConfiguration config)
        {
            _scopeFactory = scopeFactory;
            _config = config;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                await CheckExpiredLoans();
                await Task.Delay(TimeSpan.FromMinutes(1), stoppingToken);

                //await Task.Delay(TimeSpan.FromHours(24), stoppingToken);
            }
        }

        private async Task CheckExpiredLoans()
        {
            using var scope = _scopeFactory.CreateScope();

            var loanRepo = scope.ServiceProvider.GetRequiredService<ILoanRepository>();
            var emailService = scope.ServiceProvider.GetRequiredService<EmailService>();
            var fineRepo = scope.ServiceProvider.GetRequiredService<IFineRepository>();

            var now = DateTime.UtcNow;
            var loans = await loanRepo.GetActiveLoansAsync();

            var today = now.Date;
            var tomorrow = today.AddDays(1);

            foreach (var loan in loans)
            {
                var returnDate = loan.ReturnDate.Date;

                // ==============================
                // 1) RECORDATORIO UN DÍA ANTES
                // ==============================
                if (returnDate == tomorrow && !loan.ReminderEmailSent)
                {
                    string userEmail = await BuscarCorreoUsuario(loan.UserId);

                    string subject = "ReadHub: Recordatorio de Préstamo";

                    string body = @$"
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
        <h1>Tu préstamo vence mañana</h1>
    </div>

    <p>Queremos informarte que tu préstamo se vence mañana. Te recomendamos devolverlo o solicitar una renovación.</p>

    <div class='date-box'>
        Fecha de vencimiento: {loan.ReturnDate:dd/MM/yyyy}
    </div>

    <p>Gracias por utilizar ReadHub.</p>

    <div class='footer'>
        © {DateTime.UtcNow.Year} ReadHub 
    </div>
</div>

</body>
</html>";

                    await emailService.SendEmailAsync(userEmail, subject, body);

                    loan.ReminderEmailSent = true;
                    await loanRepo.SaveChangesAsync();

                    continue;
                }

                // ============================
                // 2) CORREO DE PRÉSTAMO VENCIDO
                // ============================
                if (loan.ReturnDate < now && !loan.ExpiredEmailSent)
                {
                    string userEmail = await BuscarCorreoUsuario(loan.UserId);

                    string subject = "Aviso importante: Tu préstamo ha vencido";

                    string body = @$"
<!DOCTYPE html>
<html>
<head>
<meta charset='UTF-8'>
<style>
    body {{
        background-color: #fcf1f1;
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
        <h1>⚠ Préstamo Vencido</h1>
    </div>

    <p>Tu préstamo ha vencido. Te pedimos realizar la devolución cuanto antes para evitar restricciones.</p>
    <p>Si lo necesitas, puedes solicitar una <b>renovación</b>.</p>

    <div class='date-box'>
        Venció el: {loan.ReturnDate:dd/MM/yyyy}
    </div>

    <div class='footer'>
        © {DateTime.UtcNow.Year} ReadHub 
    </div>
</div>

</body>
</html>";

                    await emailService.SendEmailAsync(userEmail, subject, body);

                    loan.ExpiredEmailSent = true;
                    await loanRepo.SaveChangesAsync();
                }

                // ============================
                // 3) GENERACIÓN DE MULTA + CORREO
                // ============================
                if (loan.ReturnDate < now)
                {
                    var daysLate = (now.Date - loan.ReturnDate.Date).Days;
                    var weekNumber = (daysLate / 7) + 1;

                    var existingFines = await fineRepo.GetFinesByUser(loan.UserId);
                    bool alreadyGenerated = existingFines.Any(f => f.LoanId == loan.Id && f.WeekNumber == weekNumber);

                    if (!alreadyGenerated)
                    {
                        decimal amount = weekNumber * 10m;

                        var fine = new Fine
                        {
                            LoanId = loan.Id,
                            UserId = loan.UserId,
                            DaysLate = daysLate,
                            WeekNumber = weekNumber,
                            Amount = amount,
                            CreatedAt = now
                        };

                        await fineRepo.AddAsync(fine);
                        await fineRepo.SaveChangesAsync();

                        // ============================
                        // CORREO DE MULTA GENERADA
                        // ============================
                        string userEmail = await BuscarCorreoUsuario(loan.UserId);

                        string subjectFine = "Multa generada por préstamo vencido";

                        string bodyFine = @$"
<!DOCTYPE html>
<html>
<head>
<meta charset='UTF-8'>
<style>
    body {{
        background-color: #fff8f0;
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
    .fine-box{{
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
        <h1> Multa generada</h1>
    </div>

    <p>Tu préstamo continúa vencido y se ha generado una multa correspondiente a la semana <b>{weekNumber}</b>.</p>

    <div class='fine-box'>
        Multa generada: <b>${amount} MXN</b><br>
        Días de retraso acumulados: {daysLate} días
    </div>

    <p>Por favor regulariza tu situación para evitar cargos adicionales.</p>

    <div class='footer'>
        © {DateTime.UtcNow.Year} ReadHub Biblioteca
    </div>
</div>

</body>
</html>";

                        await emailService.SendEmailAsync(userEmail, subjectFine, bodyFine);
                    }
                }
            }
        }

        private async Task<string> BuscarCorreoUsuario(Guid userId)
        {
            using var http = new HttpClient();

            string authApiUrl = _config["Services:AuthApi"];
            string url = $"{authApiUrl}/api/users/{userId}";

            var response = await http.GetAsync(url);

            if (!response.IsSuccessStatusCode)
                throw new Exception($"Error al obtener usuario {userId}");

            var user = await response.Content.ReadFromJsonAsync<UserDto>();

            return user.Email;
        }
    }
}
