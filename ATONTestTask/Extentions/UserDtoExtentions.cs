using ATONTestTask.ViewModels.Resposne;
using Newtonsoft.Json;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace ATONTestTask.Extentions
{
    public static class UserDtoExtentions
    {
        public static ClaimsIdentity GetIdentity(this UserDto userDto)
        {
            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(ClaimTypes.Name, userDto.Name),
                new Claim(ClaimTypes.NameIdentifier, userDto.Login),
                new Claim(ClaimTypes.Role, userDto.Admin ? "Admin" : "User")
            };

            var claimsIdentity = new ClaimsIdentity(claims, "Token", ClaimsIdentity.DefaultNameClaimType, ClaimsIdentity.DefaultRoleClaimType);
            return claimsIdentity;
        }
    }
}
