using InvoiceApp_2.Data;
using InvoiceApp_2.Model;
using Microsoft.EntityFrameworkCore;

namespace InvoiceApp_2.Services
{
    public class StatusService
    {
        private readonly AppDbContext _db;

        public StatusService(AppDbContext db)
        {
            _db = db;
        }

        public async Task<List<Status>> GetAllStatuses()
        {
            return await _db.Status.ToListAsync();
        }
    }
}
