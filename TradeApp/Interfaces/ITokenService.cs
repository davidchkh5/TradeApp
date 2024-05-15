using TradeApp.Entities;

namespace TradeApp.Interfaces
{
    public interface ITokenService
    {
        Task<string> CreateToken(AppUser user);
    }
}
