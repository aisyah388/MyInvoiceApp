using MyInvoiceApp.Shared.ViewModel;

namespace MyInvoiceApp_API.Services.Interfaces
{
    public interface IClientService
    {
        Task<List<ClientVM>> GetAllClientsAsync();
        Task<ClientVM?> GetClientByIdAsync(Guid id);
    }
}