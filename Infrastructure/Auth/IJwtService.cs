using System.Collections.Generic;
using System.Threading.Tasks;

namespace XianYu.API.Infrastructure.Auth;

public interface IJwtService
{
    Task<string> GenerateTokenAsync(IDictionary<string, object> claims);
    Task<bool> ValidateTokenAsync(string token);
}
