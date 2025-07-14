using InvoiceApp_2.Data;
using InvoiceApp_2.Model;
using InvoiceApp_2.ViewModel;
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

        public async Task<List<StatusVM>> GetAllStatuses()
        {
            return await _db.Status
               .Select(status => new StatusVM
               {
                  Id = status.Id,
                  Name = status.Name
               }).ToListAsync();
        }
        public async Task<List<StatusSummary>> GetInvoiceCountByStatus()
        {
            return await _db.Status
                .Select(status => new StatusSummary
                {
                    Id = status.Id,
                    Name = status.Name,
                    InvoiceCount = _db.Invoices.Count(i => i.Status_Id == status.Id)
                })
                .ToListAsync();
        }

        public class StatusSummary
        {
            public Guid Id { get; set; }
            public string? Name { get; set; }
            public int InvoiceCount { get; set; }
        }
    }
}
