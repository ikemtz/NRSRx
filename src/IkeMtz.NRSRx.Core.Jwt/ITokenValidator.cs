using System.Threading.Tasks;
using Microsoft.IdentityModel.Tokens;

namespace IkeMtz.NRSRx.Core.Jwt
{
  public interface ITokenValidator
  {
    TokenValidationParameters TokenValidationParameters { get; }

    Task InitAsync(string metaDataAddress, string audience);
    Task InitAsync(string metaDataAddress, string issuer, string audience);
    bool ValidateToken(string token);
  }
}
