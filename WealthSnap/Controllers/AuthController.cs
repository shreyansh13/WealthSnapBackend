using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using WealthSnap.Models;
using WealthSnap.Services;
using LoginRequest = WealthSnap.Models.LoginRequest;

namespace WealthSnap.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly JwtService _jwtService;
        private readonly UserAuthService _userService;

        public AuthController(JwtService jwtService, UserAuthService userService)
        {
            _jwtService = jwtService;
            _userService = userService;
        }

        [HttpPost("login")]
        public IActionResult Login([FromBody] LoginRequest loginRequest)
        {
            var user = _userService.Authenticate(loginRequest.Username, loginRequest.Password);
            if (user == null)
                return Unauthorized();

            var accessToken = _jwtService.GenerateAccessToken(user);
            var refreshToken = _jwtService.GenerateRefreshToken();

            user.RefreshToken = refreshToken;
            user.RefreshTokenExpiryTime = DateTime.Now.AddDays(7); // Set refresh token expiration

            _userService.UpdateUser(user);

            return Ok(new AuthenticationResponse
            {
                AccessToken = accessToken,
                RefreshToken = refreshToken
            });
        }

        [HttpPost("refresh-token")]
        public IActionResult RefreshToken([FromBody] RefreshTokenRequest tokenRequest)
        {
            var principal = _jwtService.GetPrincipalFromExpiredToken(tokenRequest.AccessToken);
            var userId = int.Parse(principal.FindFirstValue(JwtRegisteredClaimNames.Sub));

            var user = _userService.GetUserById(userId);
            if (user == null || user.RefreshToken != tokenRequest.RefreshToken || user.RefreshTokenExpiryTime <= DateTime.Now)
                return Unauthorized();

            var newAccessToken = _jwtService.GenerateAccessToken(user);
            var newRefreshToken = _jwtService.GenerateRefreshToken();

            user.RefreshToken = newRefreshToken;
            _userService.UpdateUser(user);

            return Ok(new AuthenticationResponse
            {
                AccessToken = newAccessToken,
                RefreshToken = newRefreshToken
            });
        }
    }
}
