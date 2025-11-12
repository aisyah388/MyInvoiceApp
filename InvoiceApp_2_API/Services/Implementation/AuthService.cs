using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using MyInvoiceApp.Shared.Model;
using MyInvoiceApp_API.Data;
using MyInvoiceApp_API.Services.Repository;
using MyInvoiceApp_Shared.Model;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace MyInvoiceApp_API.Services.Implementation
{
    public class AuthService : IAuthService
    {
        private readonly AppDbContext _context;
        private readonly IConfiguration _configuration;
        private readonly IValidator<Register> _registerValidator;

        public AuthService(
            AppDbContext context,
            IConfiguration configuration,
            IValidator<Register> registerValidator)
        {
            _context = context;
            _configuration = configuration;
            _registerValidator = registerValidator;
        }

        public async Task<RegisterResponse> RegisterAsync(Register request)
        {
            // Validate
            var validationResult = await _registerValidator.ValidateAsync(request);
            if (!validationResult.IsValid)
            {
                throw new ValidationException(validationResult.Errors);
            }

            // Check if email exists
            if (await EmailExistsAsync(request.Email))
            {
                throw new InvalidOperationException("Email already registered");
            }

            // Check if company registration exists
            if (await CompanyRegistrationExistsAsync(request.SSM_No))
            {
                throw new InvalidOperationException("Company registration number already exists");
            }

            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                // Create Company
                var company = new Company
                {
                    Id = Guid.NewGuid(),
                    Name = request.CompanyName,
                    SSM_No = request.SSM_No,
                    CreatedAt = DateTime.UtcNow,
                    IsActive = true
                };

                _context.Companies.Add(company);
                await _context.SaveChangesAsync();

                // Get Admin role (or create if doesn't exist)
                var adminRole = await _context.Roles.FirstOrDefaultAsync(r => r.Name == "Admin");
                if (adminRole == null)
                {
                    adminRole = new Role { Id = Guid.NewGuid(), Name = "Admin" };
                    _context.Roles.Add(adminRole);
                    await _context.SaveChangesAsync();
                }

                // Create User (as company admin)
                var user = new User
                {
                    Id = Guid.NewGuid(),
                    Username = request.Email.Split('@')[0], // Use email prefix as username
                    Email = request.Email,
                    Name = request.Name,
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password),
                    Company_Id = company.Id,
                    Role_Id = adminRole.Id,
                    CreatedAt = DateTime.UtcNow,
                    IsActive = true
                };

                _context.Users.Add(user);
                await _context.SaveChangesAsync();

                await transaction.CommitAsync();

                return new RegisterResponse
                {
                    Success = true,
                    Message = "Registration successful",
                    UserId = user.Id,
                    CompanyId = company.Id
                };
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        public async Task<LoginResponse> LoginAsync(Login request)
        {
            var user = await _context.Users
                .Include(u => u.Role)
                .Include(u => u.Company)
                .FirstOrDefaultAsync(u => u.Username == request.Username);

            if (user == null || !user.IsActive)
            {
                throw new UnauthorizedAccessException("Invalid credentials");
            }

            if (!BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
            {
                throw new UnauthorizedAccessException("Invalid credentials");
            }

            if (user.Company == null || !user.Company.IsActive)
            {
                throw new UnauthorizedAccessException("Company account is inactive");
            }

            var token = GenerateJwtToken(user);

            return new LoginResponse
            {
                Token = token,
                Username = user.Username,
                Email = user.Email,
                CompanyName = user.Company.Name,
                Role = user.Role?.Name ?? "User"
            };
        }

        public async Task<bool> EmailExistsAsync(string email)
        {
            return await _context.Users.AnyAsync(u => u.Email == email);
        }

        public async Task<bool> CompanyRegistrationExistsAsync(long SSM_No)
        {
            return await _context.Companies.AnyAsync(c => c.SSM_No == SSM_No);
        }

        private string GenerateJwtToken(User user)
        {
            var key = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(_configuration["Jwt:Key"])
            );
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.Username),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Role, user.Role?.Name ?? "User"),
                new Claim("companyName", user.Company?.Name ?? string.Empty),
                new Claim("CompanyId", user.Company_Id.ToString())
            };

            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddHours(8),
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
