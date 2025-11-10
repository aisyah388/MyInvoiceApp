using MyInvoiceApp_API.Data;
using MyInvoiceApp_API.Services.Interfaces;
using MyInvoiceApp.Shared.Model;
using MyInvoiceApp_Shared.DTO;
using MyInvoiceApp_Shared.ViewModel;
using Microsoft.EntityFrameworkCore;

namespace MyInvoiceApp_API.Services.Implementation
{
    public class InvoiceService : IInvoiceService
    {
        private readonly AppDbContext _context;

        public InvoiceService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<InvoiceDto>> GetAllInvoicesAsync()
        {
            var invoices = await _context.Invoices
                .Include(i => i.Items)
                .Include(i => i.Client)
                .Include(i => i.Status)
                .OrderByDescending(i => i.Issue_Date)
                .AsNoTracking() // Read-only optimization
                .ToListAsync();

            return invoices.Select(MapToDto).ToList();
        }

        public async Task<Invoice?> GetInvoiceByIdAsync(Guid id)
        {
            var invoice = await _context.Invoices
                .Include(i => i.Items)
                .Include(i => i.Client)
                .FirstOrDefaultAsync(i => i.Id == id);

            if (invoice != null)
            {
                invoice.Total = CalculateInvoiceTotal(invoice);
            }

            return invoice;
        }

        public async Task<Invoice> CreateInvoiceAsync(Invoice invoice)
        {
            // Invoice must have at least one item
            if (invoice.Items == null || !invoice.Items.Any())
            {
                throw new InvalidOperationException("Invoice must contain at least one item.");
            }

            // Due date cannot be before issue date
            if (invoice.Due_Date.HasValue && invoice.Issue_Date.HasValue
                && invoice.Due_Date < invoice.Issue_Date)
            {
                throw new InvalidOperationException("Due date cannot be before issue date.");
            }

            // All items must have positive quantity and price
            if (invoice.Items.Any(i => i.Quantity <= 0 || i.Unit_Price < 0))
            {
                throw new InvalidOperationException("All items must have positive quantity and non-negative price.");
            }

            invoice.Id = Guid.NewGuid();

            _context.Invoices.Add(invoice);
            await _context.SaveChangesAsync();

            return invoice;
        }

        public async Task<Invoice> UpdateInvoiceAsync(Guid id, Invoice invoice)
        {
            var existingInvoice = await _context.Invoices
                .Include(i => i.Items)
                .FirstOrDefaultAsync(i => i.Id == id);

            if (existingInvoice == null)
            {
                throw new KeyNotFoundException($"Invoice with ID {id} not found.");
            }

            // Apply same business rules as Create
            if (invoice.Items == null || !invoice.Items.Any())
            {
                throw new InvalidOperationException("Invoice must contain at least one item.");
            }

            if (invoice.Due_Date.HasValue && invoice.Issue_Date.HasValue
                && invoice.Due_Date < invoice.Issue_Date)
            {
                throw new InvalidOperationException("Due date cannot be before issue date.");
            }

            if (invoice.Items.Any(i => i.Quantity <= 0 || i.Unit_Price < 0))
            {
                throw new InvalidOperationException("All items must have positive quantity and non-negative price.");
            }

            existingInvoice.Number = invoice.Number;
            existingInvoice.Client_Id = invoice.Client_Id;
            existingInvoice.Status_Id = invoice.Status_Id;
            existingInvoice.Issue_Date = invoice.Issue_Date;
            existingInvoice.Due_Date = invoice.Due_Date;

            // Handle items - remove old, add/update new
            _context.Invoice_Items.RemoveRange(existingInvoice.Items);
            existingInvoice.Items = invoice.Items;

            await _context.SaveChangesAsync();

            return existingInvoice;
        }

        public async Task<bool> DeleteInvoiceAsync(Guid id)
        {
            var invoice = await _context.Invoices
                .Include(i => i.Items)
                .FirstOrDefaultAsync(i => i.Id == id);

            if (invoice == null)
            {
                return false;
            }

            // Cascade delete items
            if (invoice.Items != null && invoice.Items.Any())
            {
                _context.Invoice_Items.RemoveRange(invoice.Items);
            }

            _context.Invoices.Remove(invoice);
            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<string> GenerateNextInvoiceNumberAsync()
        {
            var currentYear = DateTime.UtcNow.Year;
            var prefix = $"INV-{currentYear}-";

            var lastInvoice = await _context.Invoices
                .Where(i => i.Number.StartsWith(prefix))
                .OrderByDescending(i => i.Number)
                .AsNoTracking()
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

            return $"{prefix}{nextNumber:D3}";
        }

        public async Task<List<InvoiceSummaryVM>> GetInvoiceSummaryAsync()
        {
            var invoices = await _context.Invoices
                .Include(i => i.Items)
                .Where(i => i.Issue_Date.HasValue)
                .AsNoTracking()
                .ToListAsync();

            var summaries = invoices
                .Select(i => new
                {
                    Year = i.Issue_Date!.Value.Year,
                    Month = i.Issue_Date.Value.Month,
                    Total = CalculateInvoiceTotal(i)
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

            return monthlyTotals
                .Concat(yearlyTotals)
                .OrderBy(x => x.Year)
                .ThenBy(x => x.Month ?? 0)
                .ToList();
        }

        private decimal CalculateInvoiceTotal(Invoice invoice)
        {
            return invoice.Items?.Sum(item => item.Unit_Price * item.Quantity) ?? 0m;
        }

        private InvoiceDto MapToDto(Invoice invoice)
        {
            return new InvoiceDto
            {
                Id = invoice.Id,
                Number = invoice.Number,
                Issue_Date = invoice.Issue_Date,
                Due_Date = invoice.Due_Date,
                Total = CalculateInvoiceTotal(invoice),
                Client = invoice.Client != null ? new ClientDto
                {
                    Id = invoice.Client.Id,
                    Company_Name = invoice.Client.Company_Name
                } : null,
                Status = invoice.Status != null ? new StatusDto
                {
                    Id = invoice.Status.Id,
                    Name = invoice.Status.Name
                } : null,
                Items = invoice.Items?.Select(item => new InvoiceItemDto
                {
                    Id = item.Id,
                    Description = item.Description,
                    Unit_Price = item.Unit_Price,
                    Quantity = item.Quantity
                }).ToList() ?? new List<InvoiceItemDto>()
            };
        }
    }
}