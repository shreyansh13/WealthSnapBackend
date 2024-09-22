using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace WealthSnap.Services
{
    public class TokenHandler
    {
        public static int GetUserIdFromToken(string token)
        {
            var handler = new JwtSecurityTokenHandler();
            var jwtToken = handler.ReadJwtToken(token);

            // Get the user ID from the "sub" claim
            var userIdClaim = jwtToken.Claims.FirstOrDefault(c => c.Type == "sub");
            return userIdClaim != null ? int.Parse(userIdClaim.Value) : 0; //
        }
    }
}
