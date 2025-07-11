using InvoiceApp_2.Data;
using InvoiceApp_2.Model;
using InvoiceApp_2.ViewModel;
using Microsoft.EntityFrameworkCore;

namespace InvoiceApp_2.Services
{
    public class ClientService
    {
        private readonly AppDbContext _db;

        public ClientService(AppDbContext db)
        {
            _db = db;
        }

        public async Task<List<ClientVM>> GetAllClients()
        {
            return await _db.Clients
                .Select(client => new ClientVM
                {
                    Id = client.Id,
                    Company_Name = client.Company_Name,
                    Email = client.Email,
                    Phone = client.Phone,
                    Address = client.Address
                }).ToListAsync();
        }

        public async Task<ClientVM> GetClientById(Guid id)
        {
            var client = await _db.Clients
                .FirstOrDefaultAsync(x => x.Id == id);
            if (client == null)
            {
                return null;
            }
            return new ClientVM
            {
                Company_Name = client.Company_Name,
                Email = client.Email,
                Phone = client.Phone,
                Address = client.Address
            };
        }
    }
}
