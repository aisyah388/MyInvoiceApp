using MyInvoiceApp.Shared.Model;
using MyInvoiceApp.Shared.ViewModel;

namespace MyInvoiceApp_API.Services.Repository
{
    public interface IClientService
    {
        Task<List<ClientVM>> GetAllClientsAsync();
        Task<ClientVM?> GetClientByIdAsync(Guid id);
        Task<Client> CreateClientAsync(Client client);
        Task<Client> UpdateClientAsync(Guid id, Client client);
        Task<bool> DeleteClientAsync(Guid id);
    }
}