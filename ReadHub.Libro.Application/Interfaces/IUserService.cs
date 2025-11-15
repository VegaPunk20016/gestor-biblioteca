namespace ReadHub.Libro.Application.Interfaces
{
    public interface IUserService
    {
        Task<string> GetUserEmailAsync(Guid userId);
    }
}
