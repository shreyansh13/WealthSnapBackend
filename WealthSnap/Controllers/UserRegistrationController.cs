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

            try
            {
                // Save the user to the database
                var user = await _userRegistrationService.RegisterUserAsync(userRegistrationRequest);

                // Return the user object with status code
                return Ok(new
                {
                    statusCode = StatusCodes.Status200OK,
                    message = "User registered successfully.",
                    user = new
                    {
                        Id = user.UserId,
                        Username = user.UserName,
                        Email = user.Email,
                        AccessToken = user.AccessToken,
                        RefreshToken = user.RefreshToken
                    }
                });
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new
                {
                    statusCode = StatusCodes.Status500InternalServerError,
                    message = "An error occurred while registering the user."
                });
            }

          

           
        }
    }
}
