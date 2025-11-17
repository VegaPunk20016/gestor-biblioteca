using ReadHub.Libro.Application.Interfaces;
using System.Net.Http.Json;

namespace ReadHub.Libro.Application.Services
{
    public class UserService : IUserService
    {
        private readonly HttpClient _httpClient;

        public UserService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        // Obtener el correo electrónico del usuario por su ID
        public async Task<string> GetUserEmailAsync(Guid userId)
        {
            var user = await _httpClient.GetFromJsonAsync<UserDto>($"api/users/{userId}");

            return user?.Email ?? throw new Exception("Usuario no encontrado");
        }
    }

    public record UserDto(Guid Id, string Email, string Username);
}