using MyInvoiceApp_API.Data;
using MyInvoiceApp.Shared.Model;
using MyInvoiceApp.Shared.ViewModel;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;

namespace MyInvoiceApp_API.Controller
{
    [ApiController]
    [Route("api/client")]
    public class ClientController : ControllerBase
    {
        private readonly AppDbContext _db;

        public ClientController(AppDbContext db)
        {
            _db = db;
        }

        [HttpGet("all-clients")]
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

        [HttpGet("{id}")]
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
