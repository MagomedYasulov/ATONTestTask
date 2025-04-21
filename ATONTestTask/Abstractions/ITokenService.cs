using System.Security.Claims;

namespace ATONTestTask.Abstractions
{
    public interface ITokenService
    {
        public string GenerateAccessToken(ClaimsIdentity identity);

        public string GenerateRefreshToken();

        public ClaimsPrincipal? GetPrincipalFromExpiredToken(string accessToken);
    }
}
