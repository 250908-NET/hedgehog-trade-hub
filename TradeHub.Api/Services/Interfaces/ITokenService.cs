using TradeHub.API.Models;

public interface ITokenService
{
    string GenerateToken(User user, IList<string> roles);
 }