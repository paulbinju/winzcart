using Winzcart.Domain.Entities;

namespace Winzcart.Application.Interfaces.Services;

public interface IJwtTokenService
{
    string GenerateToken(AppUser user);
}
