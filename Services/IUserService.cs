using pharmacy_management.Models;

namespace pharmacy_management.Services
{
    public interface IUserService
    {
        Task<List<UserListDto>> GetAllUsersAsync();

        Task<UserListDto?> GetUserById(Guid id);

        Task<LoginResponseDto?> LoginAsync(UserLoginDto request);

        Task<UserListDto?> RegisterAsync(UserCreateDto request);

        Task<UserCreateDto?> UpdateUserAsync(Guid id, UserCreateDto request);

        Task<bool?> DeleteUserAsync(Guid id);

        Task<UserListDto?> ResetPasswordAsync(ResetPasswordDto request);

        Task<bool?> SendOtpAsync(SendOtpDto request);
        
        Task<bool?> VerifyOtpAsync(VerifyOtpDto request);
    }
}