using ReadHub.Libro.Application.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;

namespace ReadHub.Libro.Application.Services
{
    public class UserService : IUserService
    {
        private readonly HttpClient _httpClient;
        public UserService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<string> GetUserEmailAsync(Guid userId)
        {
            var user = await _httpClient.GetFromJsonAsync<UserDto>($"/api/users/{userId}");
            return user?.Email ?? throw new Exception("Usuario no encontrado");
        }
    }

    public record UserDto(Guid Id, string Email, string Username);
}