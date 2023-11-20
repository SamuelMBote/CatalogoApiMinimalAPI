using CatalogoApiMinimalAPI.Models;

namespace CatalogoApiMinimalAPI.Services
{
    public interface ITokenService
    {
        string GerarToken(string key, string issuer, string audience, User user);
    }
}
