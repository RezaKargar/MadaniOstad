using MadaniOstad.Entities.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Threading.Tasks;

namespace MadaniOstad.Services.Services
{
    public interface IJwtService
    {
        Task<AccessToken> GenerateAsync(User user);

        JwtSecurityToken ReadToken(string token);
        
        string GetIdFromToken(string token);
    }
}
