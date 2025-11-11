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


        public async Task<List<ClientVM>> GetAllClientsAsync()
        {
            return await _context.Clients
                .AsNoTracking()
                .Select(client => new ClientVM
                {
                    Id = client.Id,
                    Name = client.Name,
                    Email = client.Email,
                    Phone = client.Phone,
                    Address = client.Address
                })
                .OrderBy(c => c.Name)
                .ToListAsync();
        }

        public async Task<ClientVM?> GetClientByIdAsync(Guid id)
        {
            var client = await _context.Clients
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == id);

            if (client == null)
            {
                return null;
            }

            return new ClientVM
            {
                Id = client.Id,
                Name = client.Name,
                Email = client.Email,
                Phone = client.Phone,
                Address = client.Address
            };
        }

        public async Task<Client> CreateClientAsync(Client client)
        {
            // Validate using FluentValidation
            var validationResult = await _validator.ValidateAsync(client);
            if (!validationResult.IsValid)
            {
                throw new ValidationException(validationResult.Errors);
            }

            client.Id = Guid.NewGuid();

            _context.Clients.Add(client);
            await _context.SaveChangesAsync();

            return client;
        }

        public async Task<Client> UpdateClientAsync(Guid id, Client client)
        {
            var existingClient = await _context.Clients
                .FirstOrDefaultAsync(i => i.Id == id);

            if (existingClient == null)
            {
                throw new KeyNotFoundException($"Client name: {client.Name} not found.");
            }

            // Validate using FluentValidation
            var validationResult = await _validator.ValidateAsync(client);
            if (!validationResult.IsValid)
            {
                throw new ValidationException(validationResult.Errors);
            }

            // Update client properties
            existingClient.Name = client.Name;
            existingClient.Address = client.Address;
            existingClient.Phone = client.Phone;
            existingClient.Email = client.Email;

            await _context.SaveChangesAsync();

            return existingClient;
        }

        public async Task<bool> DeleteClientAsync(Guid id)
        {
            var client = await _context.Clients
                .FirstOrDefaultAsync(i => i.Id == id);

            if (client == null)
            {
                return false;
            }

            _context.Clients.Remove(client);
            await _context.SaveChangesAsync();

            return true;
        }
    }
}