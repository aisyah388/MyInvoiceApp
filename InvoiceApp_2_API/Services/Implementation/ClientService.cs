using MyInvoiceApp_API.Data;
using MyInvoiceApp_API.Services.Interfaces;
using MyInvoiceApp.Shared.ViewModel;
using Microsoft.EntityFrameworkCore;

namespace MyInvoiceApp_API.Services.Implementation
{
    public class ClientService : IClientService
    {
        private readonly AppDbContext _context;

        public ClientService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<ClientVM>> GetAllClientsAsync()
        {
            return await _context.Clients
                .AsNoTracking()
                .Select(client => new ClientVM
                {
                    Id = client.Id,
                    Company_Name = client.Company_Name,
                    Email = client.Email,
                    Phone = client.Phone,
                    Address = client.Address
                })
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
                Company_Name = client.Company_Name,
                Email = client.Email,
                Phone = client.Phone,
                Address = client.Address
            };
        }
    }
}