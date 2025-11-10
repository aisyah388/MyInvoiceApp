using MyInvoiceApp.Shared.ViewModel;

namespace MyInvoiceApp_API.Services.Repository
{
    public interface IClientService
    {
        Task<List<ClientVM>> GetAllClientsAsync();
        Task<ClientVM?> GetClientByIdAsync(Guid id);
    }
}