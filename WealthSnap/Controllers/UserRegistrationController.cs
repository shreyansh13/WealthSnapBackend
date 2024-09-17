using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using WealthSnap.Models;
using WealthSnap.Services;
using BCrypt.Net;

namespace WealthSnap.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserRegistrationController : ControllerBase
    {
        private readonly IUserRegistrationService _userRegistrationService;

        public UserRegistrationController(IUserRegistrationService userService)
        {
            _userRegistrationService = userService;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(UserRegistrationRequest request)
        {
            // Check if the email or username is already taken
            if (await _userRegistrationService.UserExistsAsync(request.Email, request.UserName))
            {
                return BadRequest(new { message = "Email or Username is already in use." });
            }

            var userRegistrationRequest = new UserRegistrationRequest
            {
                UserName = request.UserName,
                Email = request.Email,
                Password = request.Password,
            };

            // Save the user to the database
            var result = await _userRegistrationService.RegisterUserAsync(userRegistrationRequest);

            if (!result)
            {
                return StatusCode(500, "An error occurred while registering the user.");
            }

            return Ok(new { message = "User registered successfully." });
        }
    }
}
