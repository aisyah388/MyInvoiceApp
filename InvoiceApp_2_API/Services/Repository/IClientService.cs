using MyInvoiceApp.Shared.Model;
using MyInvoiceApp.Shared.ViewModel;

namespace MyInvoiceApp_API.Services.Repository
{
    public interface IClientService
    {
        Task<List<ClientVM>> GetAllClientsAsync(Guid companyId);
        Task<ClientVM?> GetClientByIdAsync(Guid id, Guid companyId);
        Task<Client> CreateClientAsync(Client client, Guid companyId);
        Task<Client> UpdateClientAsync(Guid id, Client client, Guid companyId);
        Task<bool> DeleteClientAsync(Guid id, Guid companyId);
    }
}