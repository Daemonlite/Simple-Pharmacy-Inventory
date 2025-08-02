using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using pharmacy_management.Data;
using pharmacy_management.Entities;
using pharmacy_management.Models;
using pharmacy_management.Exceptions;

namespace pharmacy_management.Services
{
    public class UserService(AppDbContext context, IConfiguration configuration, EmailService emailService, RedisCacheService cache) : IUserService
    {

        public async Task<List<UserListDto>> GetAllUsersAsync()
        {
            var users = await context.Users
                .Select(u => new UserListDto
                {
                    Id = u.Id,
                    Name = u.Name,
                    Email = u.Email,
                    Role = u.Role.ToString(),
                    CreatedAt = u.CreatedAt
                })
                .ToListAsync();

            return users;
        }

        public async Task<UserListDto?> GetUserById(Guid id)
        {
            return await context.Users
                .Where(u => u.Id == id)
                .Select(u => new UserListDto
                {
                    Id = u.Id,
                    Name = u.Name,
                    Email = u.Email,
                    Role = u.Role.ToString(),
                    CreatedAt = u.CreatedAt
                })
                .FirstOrDefaultAsync();
        }

        public async Task<LoginResponseDto?> LoginAsync(UserLoginDto request)
        {
            if (request == null) throw new ArgumentNullException(nameof(request));

            var user = await context.Users
                .FirstOrDefaultAsync(u => u.Email == request.Email.ToLower().Trim()) ?? throw new UserNotFoundException(request.Email);

            var passwordHasher = new PasswordHasher<User>();
            var verificationResult = passwordHasher.VerifyHashedPassword(user, user.PasswordHash, request.Password);

            if (verificationResult == PasswordVerificationResult.Failed)
            {
                throw new InvalidCredentialsException();
            }

            var token = CreateToken(user);

            return new LoginResponseDto
            {
                Token = token,
                Data = new UserDto
                {
                    Id = user.Id,
                    Name = user.Name,
                    Email = user.Email,
                    Role = user.Role.ToString(),
                }
            };
        }

        public async Task<UserListDto?> RegisterAsync(UserCreateDto request)
        {
            ArgumentNullException.ThrowIfNull(request);

            var user = await context.Users
                .FirstOrDefaultAsync(u => u.Email == request.Email.ToLower().Trim());

            if (user != null) throw new UserAlreadyExistsException(request.Email);


            var passwordHasher = new PasswordHasher<User>();
            var hashedPassword = passwordHasher.HashPassword(user?? new User(), request.Password);

            var newUser = new User
            {
                Name = request.Name,
                Email = request.Email.ToLower().Trim(),
                PasswordHash = hashedPassword,
                Role = Enum.Parse<Role>(request.Role)
            };

            await context.Users.AddAsync(newUser);
            await context.SaveChangesAsync();

            return new UserListDto
            {
                Id = newUser.Id,
                Name = newUser.Name,
                Email = newUser.Email,
                Role = newUser.Role.ToString(),
                CreatedAt = newUser.CreatedAt
            };
        }

        public async Task<UserListDto?> ResetPasswordAsync(ResetPasswordDto request)
        {
            if (request == null) throw new ArgumentNullException(nameof(request));

            if (request.Password != request.ConfirmPassword) throw new PasswordsDoNotMatchException();

            var user = await context.Users
                .FirstOrDefaultAsync(u => u.Email == request.Email.ToLower().Trim()) ?? throw new UserNotFoundException(request.Email);

            var passwordHasher = new PasswordHasher<User>();
            var hashedPassword = passwordHasher.HashPassword(user, request.Password);

            user.PasswordHash = hashedPassword;
            await context.SaveChangesAsync();
            await  emailService.SendEmailAsync(request.Email, "OTP Verification Success", $@"
                <!DOCTYPE html>
                <html>
                    <head>
                        <style>
                            body {{ font-family: Arial, sans-serif; line-height: 1.6; color: #333; max-width: 600px; margin: 0 auto; padding: 20px; }}
                            .header {{ color: #2c3e50; border-bottom: 1px solid #eee; padding-bottom: 10px; }}
                            .otp-container {{ background: #f9f9f9; padding: 20px; text-align: center; margin: 20px 0; border-radius: 5px; }}
                            .otp-code {{ font-size: 24px; font-weight: bold; letter-spacing: 3px; color: #2c3e50; }}
                            .footer {{ margin-top: 20px; font-size: 12px; color: #7f8c8d; text-align: center; }}
                            .warning {{ color: #e74c3c; font-weight: bold; }}
                        </style>
                    </head>
                    <body>
                        <div class='header'>
                            <h2>One-Time Password (OTP)</h2>
                        </div>
                        
                        <p>Hello {request.Email.Split('@')[0]},</p>
                        
                        <p>You Have Successfullly Reset Your Password.</p>
                        <p> If you did not initiate this password reset then contact us at <a href='mailto:info@prostash.site'>info@prostash.site</a></p>
                    
                        <div class='footer'>
                            <p>© {DateTime.Now.Year} Prostash. All rights reserved.</p>
                        </div>
                    </body>
                    </html>
                ");

            return new UserListDto
            {
                Id = user.Id,
                Name = user.Name,
                Email = user.Email,
                Role = user.Role.ToString(),
                CreatedAt = user.CreatedAt
            };
        }

        public async Task<UserCreateDto?> UpdateUserAsync(Guid id, UserCreateDto request)
        {

            var user = await context.Users
                .FirstOrDefaultAsync(u => u.Id == id) ?? throw new UserNotFoundException(id.ToString());

            user.Name = request.Name;
            user.Email = request.Email.ToLower().Trim();
            user.Role = Enum.Parse<Role>(request.Role);

            await context.SaveChangesAsync();

            return new UserCreateDto
            {
                Name = user.Name,
                Email = user.Email,
                Role = user.Role.ToString()
            };
        }

        public async Task<bool?> DeleteUserAsync(Guid id)
        {
            var user = await context.Users.FindAsync(id);
            if (user != null)
            {
                context.Users.Remove(user);
                await context.SaveChangesAsync();
                return true;
            }
            return null;
        }

        public async Task<bool?> SendOtpAsync(SendOtpDto request)
        {
            var user = await context.Users.FirstOrDefaultAsync(u => u.Email == request.Email);
            if (user != null)
            {
                var otp = GenerateOtp();
                await cache.CacheSetAsync(request.Email, otp);
                await emailService.SendEmailAsync(request.Email, "OTP", $@"
                    <!DOCTYPE html>
                    <html>
                    <head>
                        <style>
                            body {{ font-family: Arial, sans-serif; line-height: 1.6; color: #333; max-width: 600px; margin: 0 auto; padding: 20px; }}
                            .header {{ color: #2c3e50; border-bottom: 1px solid #eee; padding-bottom: 10px; }}
                            .otp-container {{ background: #f9f9f9; padding: 20px; text-align: center; margin: 20px 0; border-radius: 5px; }}
                            .otp-code {{ font-size: 24px; font-weight: bold; letter-spacing: 3px; color: #2c3e50; }}
                            .footer {{ margin-top: 20px; font-size: 12px; color: #7f8c8d; text-align: center; }}
                            .warning {{ color: #e74c3c; font-weight: bold; }}
                        </style>
                    </head>
                    <body>
                        <div class='header'>
                            <h2>One-Time Password (OTP)</h2>
                        </div>
                        
                        <p>Hello {user.Name},</p>
                        
                        <p>Your one-time password for verification is:</p>
                        
                        <div class='otp-container'>
                            <div class='otp-code'>{otp}</div>
                        </div>
                        
                        <p class='warning'>This OTP is valid for 5 minutes. Please do not share this code with anyone.</p>
                        
                        <p>If you didn't request this OTP, please ignore this email or contact support at <a href='mailto:info@prostash.site'>info@prostash.site</a>.</p>
                        
                        <div class='footer'>
                            <p>© {DateTime.Now.Year} Prostash. All rights reserved.</p>
                        </div>
                    </body>
                    </html>");
                return true;

            }
            throw new UserNotFoundException(request.Email);
            
        }

        public async Task<bool?> VerifyOtpAsync(VerifyOtpDto request)
        {
            var otp = await cache.CacheGetAsync(request.Email);
            Console.WriteLine($"OTP: {otp}");
            if (otp == request.Otp)
            {
                return true;
            }
            return false;
        }

        private string CreateToken(User user)
        {
            if (user != null)
            {
                var claims = new List<Claim>
                {
                    new(ClaimTypes.Name, user.Name),
                    new(ClaimTypes.NameIdentifier, user.Id.ToString()),
                    new(ClaimTypes.Role, user.Role.ToString())
                };

                var key = new SymmetricSecurityKey(
                    Encoding.UTF8.GetBytes(configuration.GetValue<string>("AppSettings:Token")!
                    ));

                var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512);
                var tokenDescriptor = new JwtSecurityToken(
                    issuer: configuration.GetValue<string>("AppSettings:Issuer"),
                    audience: configuration.GetValue<string>("AppSettings:Audience"),
                    claims: claims,
                    expires: DateTime.UtcNow.AddDays(1),
                    signingCredentials: creds
                );
                return new JwtSecurityTokenHandler().WriteToken(tokenDescriptor);
            }
            return string.Empty;
        }
        
        private static string GenerateOtp()
        {
            var randomNumber = new byte[6];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(randomNumber);
            return Convert.ToBase64String(randomNumber);
        }
    }
}