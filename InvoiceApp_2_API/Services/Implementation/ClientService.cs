using MyInvoiceApp_API.Data;
using MyInvoiceApp.Shared.ViewModel;
using Microsoft.EntityFrameworkCore;
using MyInvoiceApp_API.Services.Repository;
using MyInvoiceApp.Shared.Model;
using FluentValidation;
using MyInvoiceApp_Shared.ViewModel;

namespace MyInvoiceApp_API.Services.Implementation
{
    public class ClientService : IClientService
    {
        private readonly AppDbContext _context;
        private readonly IValidator<Client> _validator;

        public ClientService(AppDbContext context, IValidator<Client> validator)
        {
            _context = context;
            _validator = validator;
        }

        // ✅ Filter by company
        public async Task<List<ClientVM>> GetAllClientsAsync(Guid companyId)
        {
            return await _context.Clients
                .Where(c => c.Company_Id == companyId) // filter here
                .AsNoTracking()
                .OrderBy(c => c.Name)
                .Select(client => new ClientVM
                {
                    Id = client.Id,
                    Name = client.Name,
                    SSM_No = client.SSM_No,
                    Email = client.Email,
                    Phone = client.Phone,
                    Address = client.Address
                })
                .ToListAsync();
        }

        // ✅ Filter by company
        public async Task<ClientVM?> GetClientByIdAsync(Guid id, Guid companyId)
        {
            var client = await _context.Clients
                .Where(c => c.Company_Id == companyId)
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == id);

            if (client == null)
                return null;

            return new ClientVM
            {
                Id = client.Id,
                Name = client.Name,
                SSM_No = client.SSM_No,
                Email = client.Email,
                Phone = client.Phone,
                Address = client.Address
            };
        }

        // ✅ Assign company on creation
        public async Task<Client> CreateClientAsync(Client client, Guid companyId)
        {
            // Validate using FluentValidation
            var validationResult = await _validator.ValidateAsync(client);
            if (!validationResult.IsValid)
                throw new ValidationException(validationResult.Errors);

            client.Id = Guid.NewGuid();
            client.Company_Id = companyId; // attach company here

            _context.Clients.Add(client);
            await _context.SaveChangesAsync();

            return client;
        }

        // ✅ Update with company validation
        public async Task<Client> UpdateClientAsync(Guid id, Client client, Guid companyId)
        {
            var existingClient = await _context.Clients
                .Where(c => c.Company_Id == companyId)
                .FirstOrDefaultAsync(i => i.Id == id);

            if (existingClient == null)
                throw new KeyNotFoundException($"Client not found for company {companyId}.");

            var validationResult = await _validator.ValidateAsync(client);
            if (!validationResult.IsValid)
                throw new ValidationException(validationResult.Errors);

            existingClient.Name = client.Name;
            existingClient.Address = client.Address;
            existingClient.Phone = client.Phone;
            existingClient.Email = client.Email;

            await _context.SaveChangesAsync();
            return existingClient;
        }

        // ✅ Delete with company validation
        public async Task<bool> DeleteClientAsync(Guid id, Guid companyId)
        {
            var client = await _context.Clients
                .Where(c => c.Company_Id == companyId)
                .FirstOrDefaultAsync(i => i.Id == id);

            if (client == null)
                return false;

            _context.Clients.Remove(client);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
