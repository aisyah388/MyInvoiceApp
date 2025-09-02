using MyInvoiceApp.Shared.Model;
using MyInvoiceApp.Shared.ViewModel;
using MyInvoiceApp_API.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using MyInvoiceApp_Shared.ViewModel;

namespace MyInvoiceApp_API.Controller
{
    [ApiController]
    [Route("api/status")]
    public class StatusController : ControllerBase
    {
        private readonly AppDbContext _db;

        public StatusController(AppDbContext db)
        {
            _db = db;
        }

        [HttpGet("all-statuses")]
        public async Task<List<StatusVM>> GetAllStatuses()
        {
            return await _db.Status
               .Select(status => new StatusVM
               {
                  Id = status.Id,
                  Name = status.Name
               }).ToListAsync();
        }

        [HttpGet("invoice-count")]
        public async Task<List<StatusSummaryVM>> GetInvoiceCountByStatus()
        {
            return await _db.Status
                .Select(status => new StatusSummaryVM
                {
                    Id = status.Id,
                    Name = status.Name,
                    InvoiceCount = _db.Invoices.Count(i => i.Status_Id == status.Id)
                })
                .ToListAsync();
        }
    }
}
