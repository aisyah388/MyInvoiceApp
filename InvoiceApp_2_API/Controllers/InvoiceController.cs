using MyInvoiceApp_API.Data;
using MyInvoiceApp.Shared.Model;
using MyInvoiceApp.Shared.ViewModel;
using Microsoft.EntityFrameworkCore;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Mvc;
using MyInvoiceApp_Shared.ViewModel;

namespace MyInvoiceApp_API.Controller
{
    [ApiController]
    [Route("api/invoice")]
    public class InvoiceController : ControllerBase
    {
        private readonly AppDbContext _db;


        public InvoiceController(AppDbContext db)
        {
            _db = db;
        }

        [HttpGet("all-invoices")]
        public async Task<List<Invoice>> GetAllInvoices()
        {
            var invoices = await _db.Invoices
                .Include(i => i.Items)
                .Include(i => i.Client)
                .Include(i => i.Status)
                .OrderByDescending(i => i.Issue_Date)
                .ToListAsync();

            foreach (var invoice in invoices)
            {
                invoice.Total = invoice.Items?.Sum(item => item.Unit_Price * item.Quantity) ?? 0m;
            }

            return invoices;
        }

        [HttpGet("{id}", Name = "get-invoice-by-id")]
        public async Task<Invoice> GetInvoiceById(Guid id)
        {
            var invoice = await _db.Invoices
                .Include(i => i.Items)
                .Include(i => i.Client)
                .FirstOrDefaultAsync(i => i.Id == id);

            if (invoice != null)
            {
                invoice.Total = invoice.Items?.Sum(item => item.Unit_Price * item.Quantity) ?? 0m;
            }

            return invoice;
        }

        [HttpPost("add", Name = "add-invoice")]
        public async Task SaveInvoice(Invoice invoice)
        {
            var existingInvoice = await _db.Invoices
                .Include(i => i.Items)
                .FirstOrDefaultAsync(i => i.Id == invoice.Id);

            if (existingInvoice == null)
            {
                _db.Invoices.Add(invoice);
            }
            else
            {
                _db.Entry(existingInvoice).CurrentValues.SetValues(invoice);

                if (existingInvoice.Client != null && invoice.Client != null)
                {
                    _db.Entry(existingInvoice.Client).CurrentValues.SetValues(invoice.Client);
                }

                foreach (var existingItem in existingInvoice.Items.ToList())
                {
                    if (!invoice.Items.Any(i => i.Id == existingItem.Id))
                    {
                        _db.Remove(existingItem);
                    }
                }

                foreach (var item in invoice.Items)
                {
                    var existingItem = existingInvoice.Items.FirstOrDefault(i => i.Id == item.Id);
                    if (existingItem == null)
                    {
                        existingInvoice.Items.Add(item);
                    }
                    else
                    {
                        _db.Entry(existingItem).CurrentValues.SetValues(item);
                    }
                }
            }

            await _db.SaveChangesAsync();
        }

        [HttpDelete("{id}", Name = "delete-invoice")]
        public async Task DeleteInvoice(Guid id)
        {
            var invoice = await _db.Invoices
                .Include(i => i.Items)
                .FirstOrDefaultAsync(i => i.Id == id);

            if (invoice == null)
                throw new KeyNotFoundException("Invoice not found");

            if (invoice.Items != null)
            {
                _db.Invoice_Items.RemoveRange(invoice.Items);
            }

            _db.Invoices.Remove(invoice);
            await _db.SaveChangesAsync();
        }

        [HttpGet("next-inv-number")]
        public async Task<string> GetNextInvoiceNumber()
        {
            var currentYear = DateTime.Now.Year;

            var lastInvoice = await _db.Invoices
                .Where(i => i.Number.StartsWith($"INV-{currentYear}-"))
                .OrderByDescending(i => i.Number)
                .FirstOrDefaultAsync();

            int nextNumber = 1;

            if (lastInvoice != null)
            {
                var parts = lastInvoice.Number.Split('-');
                if (parts.Length == 3 && int.TryParse(parts[2], out int lastNumber))
                {
                    nextNumber = lastNumber + 1;
                }
            }

            return $"INV-{currentYear}-{nextNumber.ToString("D3")}";
        }

        [HttpGet("summary")]
        public async Task<List<InvoiceSummaryVM>> GetInvoiceSummary()
        {
            var invoices = await _db.Invoices
                .Include(i => i.Items)
                .Where(i => i.Issue_Date.HasValue)
                .ToListAsync();

            var summaries = invoices
                .Select(i => new
                {
                    Year = i.Issue_Date.Value.Year,
                    Month = i.Issue_Date.Value.Month,
                    Total = i.Items?.Sum(item => item.Unit_Price * item.Quantity) ?? 0m
                })
                .ToList();

            var monthlyTotals = summaries
                .GroupBy(x => new { x.Year, x.Month })
                .Select(g => new InvoiceSummaryVM
                {
                    Year = g.Key.Year,
                    Month = g.Key.Month,
                    Total = g.Sum(x => x.Total)
                });

            var yearlyTotals = summaries
                .GroupBy(x => x.Year)
                .Select(g => new InvoiceSummaryVM
                {
                    Year = g.Key,
                    Month = null,
                    Total = g.Sum(x => x.Total)
                });

            return monthlyTotals.Concat(yearlyTotals).OrderBy(x => x.Year).ThenBy(x => x.Month ?? 0).ToList();
        }
    }
}
