using MyInvoiceApp_Shared.Model;

namespace MyInvoiceApp_API.Services.Repository
{
    public interface IAuthService
    {
        Task<RegisterResponse> RegisterAsync(Register request);
        Task<LoginResponse> LoginAsync(Login request);
        Task<bool> EmailExistsAsync(string email);
        Task<bool> CompanyRegistrationExistsAsync(long SSM_No);
    }
}
