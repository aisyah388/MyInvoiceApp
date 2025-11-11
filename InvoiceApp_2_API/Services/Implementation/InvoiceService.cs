using MyInvoiceApp_API.Data;
using MyInvoiceApp_API.Services.Repository;
using MyInvoiceApp.Shared.Model;
using MyInvoiceApp_Shared.ViewModel;
using Microsoft.EntityFrameworkCore;
using FluentValidation;
using MyInvoiceApp.Shared.ViewModel;

namespace MyInvoiceApp_API.Services.Implementation
{
    public class InvoiceService : IInvoiceService
    {
        private readonly AppDbContext _context;
        private readonly IValidator<Invoice> _validator;

        public InvoiceService(AppDbContext context, IValidator<Invoice> validator)
        {
            _context = context;
            _validator = validator;
        }

        public async Task<List<InvoiceVM>> GetAllInvoicesAsync()
        {
            var invoices = await _context.Invoices
                .Include(i => i.Items)
                .Include(i => i.Client)
                .Include(i => i.Status)
                .OrderByDescending(i => i.Issue_Date)
                .AsNoTracking()
                .ToListAsync();

            return invoices.Select(MapToVM).ToList();
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
            // Validate using FluentValidation
            var validationResult = await _validator.ValidateAsync(invoice);
            if (!validationResult.IsValid)
            {
                throw new ValidationException(validationResult.Errors);
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

            // Validate using FluentValidation
            var validationResult = await _validator.ValidateAsync(invoice);
            if (!validationResult.IsValid)
            {
                throw new ValidationException(validationResult.Errors);
            }

            // Update invoice properties
            existingInvoice.Number = invoice.Number;
            existingInvoice.Client_Id = invoice.Client_Id;
            existingInvoice.Status_Id = invoice.Status_Id;
            existingInvoice.Issue_Date = invoice.Issue_Date;
            existingInvoice.Due_Date = invoice.Due_Date;

            // Handle items - remove old, add new
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

        private InvoiceVM MapToVM(Invoice invoice)
        {
            return new InvoiceVM
            {
                Id = invoice.Id,
                Number = invoice.Number,
                Issue_Date = invoice.Issue_Date,
                Due_Date = invoice.Due_Date,
                Total = CalculateInvoiceTotal(invoice),
                Client = invoice.Client != null ? new InvoiceClientVM
                {
                    Name = invoice.Client.Name
                } : null,
                Status = invoice.Status != null ? new InvoiceStatusVM
                {
                    Name = invoice.Status.Name
                } : null,
                Items = invoice.Items?.Select(item => new Invoice_ItemVM
                {
                    Description = item.Description,
                    Unit_Price = item.Unit_Price,
                    Quantity = item.Quantity
                }).ToList() ?? new List<Invoice_ItemVM>()
            };
        }
    }
}