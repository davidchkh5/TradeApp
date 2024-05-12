using TradeApp.Entities;

namespace TradeApp.Interfaces
{
    public interface ITokenService
    {
        string CreateToken(AppUser user);
    }
}
