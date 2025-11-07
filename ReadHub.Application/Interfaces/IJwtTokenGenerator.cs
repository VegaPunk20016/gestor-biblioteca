using ReadHub.Domain.Entities;

namespace ReadHub.Application.Interfaces
{
    public interface IJwtTokenGenerator
    {
        string GenerateToken(User user);
    }
}
