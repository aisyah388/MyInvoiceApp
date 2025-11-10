using MyInvoiceApp_API.Data;
using MyInvoiceApp_API.Services.Interfaces;
using MyInvoiceApp.Shared.ViewModel;
using MyInvoiceApp_Shared.ViewModel;
using Microsoft.EntityFrameworkCore;

namespace MyInvoiceApp_API.Services.Implementation
{
    public class StatusService : IStatusService
    {
        private readonly AppDbContext _context;

        public StatusService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<StatusVM>> GetAllStatusesAsync()
        {
            return await _context.Status
                .AsNoTracking()
                .Select(status => new StatusVM
                {
                    Id = status.Id,
                    Name = status.Name
                })
                .ToListAsync();
        }

        public async Task<List<StatusSummaryVM>> GetInvoiceCountByStatusAsync()
        {
            return await _context.Status
                .AsNoTracking()
                .Select(status => new StatusSummaryVM
                {
                    Id = status.Id,
                    Name = status.Name,
                    InvoiceCount = _context.Invoices.Count(i => i.Status_Id == status.Id)
                })
                .ToListAsync();
        }
    }
}