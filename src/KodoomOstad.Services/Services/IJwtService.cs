using KodoomOstad.Entities.Models;
using System.Threading.Tasks;

namespace KodoomOstad.Services.Services
{
    public interface IJwtService
    {
        Task<AccessToken> GenerateAsync(User user);
    }
}
