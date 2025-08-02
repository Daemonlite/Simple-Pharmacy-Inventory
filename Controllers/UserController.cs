

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using pharmacy_management.Services;
using pharmacy_management.Models;
using pharmacy_management.Exceptions;


namespace pharmacy_management.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController(IUserService authservice) : ControllerBase
    {
        [HttpGet]
        [Authorize]
        public async Task<ActionResult<List<UserListDto>>> GetAllUsersAsync()
        {
            return Ok(await authservice.GetAllUsersAsync());
        }

        [HttpGet("{id}")]
        [Authorize]
        public async Task<ActionResult<UserListDto>> GetUser(Guid id)
        {
            try
            {
                return Ok(await authservice.GetUserById(id));
            }
            catch (UserNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }

        }

        [HttpPost("register")]
        public async Task<ActionResult<UserListDto>> RegisterAsync(UserCreateDto request)
        {
            try
            {
                return Ok(await authservice.RegisterAsync(request));
            }
            catch (UserAlreadyExistsException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPost("login")]
        public async Task<ActionResult<LoginResponseDto>> LoginAsync(UserLoginDto request)
        {
            try
            {
                return Ok(await authservice.LoginAsync(request));
            }
            catch (UserNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (InvalidCredentialsException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPut("{id}")]
        [Authorize]
        public async Task<ActionResult<UserCreateDto>> UpdateUserAsync(Guid id, UserCreateDto request)
        {
            try
            {
                return Ok(await authservice.UpdateUserAsync(id, request));
            }
            catch (UserNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }

        [HttpDelete("{id}")]
        [Authorize]
        public async Task<ActionResult<bool>> DeleteUserAsync(Guid id)
        {
            try
            {
                return Ok(await authservice.DeleteUserAsync(id));
            }
            catch (UserNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }

        [HttpPost("forgot-password")]
        public async Task<ActionResult<UserListDto>> ForgotPasswordAsync(SendOtpDto request)
        {
            try
            {
                if (await authservice.SendOtpAsync(request) == false) return BadRequest(new { message = "Failed to send OTP" });
                return Ok(new { message = "OTP sent successfully" });
            }
            catch (UserNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }

        [HttpPost("verify-otp")]
        public async Task<ActionResult<bool>> VerifyOtpAsync(VerifyOtpDto request)
        {
            try
            {
                var result = await authservice.VerifyOtpAsync(request);
                if (result == false) return BadRequest(new { message = "Invalid OTP or Otp Expired" });
                return Ok(new { message = "OTP verified successfully" });
            }
            catch (UserNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }
        
        [HttpPost("reset-password")]
        public async Task<ActionResult<UserListDto>> ResetPasswordAsync(ResetPasswordDto request)
        {
            try
            {
                return Ok(await authservice.ResetPasswordAsync(request));
            }
            catch (UserNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (PasswordsDoNotMatchException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

    }
}