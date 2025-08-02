using System.ComponentModel.DataAnnotations;

namespace pharmacy_management.Models
{
    public class UserCreateDto
    {
        [Required(ErrorMessage = "Email is required")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Name is required")]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "Password is required")]
        public string Password { get; set; } = string.Empty;

        [Required(ErrorMessage = "Role is required")]
        public string Role { get; set; } = string.Empty;
    }

    public class UserListDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }

    public class UserLoginDto
    {
        [Required(ErrorMessage = "Email is required")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Password is required")]
        public string Password { get; set; } = string.Empty;

    }

    public class LoginResponseDto
    {
        public string Token { get; set; } = string.Empty;
        public UserDto? Data { get; set; }
    }

    public class UserDto
    {
        public Guid Id { get; set; }
        public string Email { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;

        public string Role { get; set; } = string.Empty;
    }

    public class ResetPasswordDto
    {
        [Required(ErrorMessage = "Email is required")]
        public string Email { get; set; } = string.Empty;
        [Required(ErrorMessage = "Password is required")]
        public string Password { get; set; } = string.Empty;
        [Required(ErrorMessage = "Confirm password is required")]
        public string ConfirmPassword { get; set; } = string.Empty;
    }

    public class SendOtpDto
    {
        [Required(ErrorMessage = "Email is required")]
        public string Email { get; set; } = string.Empty;
    }

    public class VerifyOtpDto
    {
        [Required(ErrorMessage = "Email is required")]
        public string Email { get; set; } = string.Empty;
        [Required(ErrorMessage = "Otp is required")]
        public string Otp { get; set; } = string.Empty;
    }

    public class FinalUploadDto
    {
        public string AbsoluteUrl { get; set; } = string.Empty;

        public string ContentType { get; set; } = string.Empty;

        public int FileSize { get; set; }

        public string FileName { get; set; } = string.Empty;
    }
}