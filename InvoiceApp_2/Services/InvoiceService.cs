using InvoiceApp_2.Data;
using InvoiceApp_2.Model;
using Microsoft.EntityFrameworkCore;
using System.Text.RegularExpressions;

namespace InvoiceApp_2.Services
{
    public class InvoiceService
    {
        private readonly AppDbContext _db;

        public InvoiceService(AppDbContext db)
        {
            _db = db;
        }

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

        public async Task SaveInvoice(Invoice invoice)
        {
            var existingInvoice = await _db.Invoices
                .Include(i => i.Items)
                .FirstOrDefaultAsync(i => i.Id == invoice.Id);

            if (existingInvoice == null)
            {
                // New invoice
                _db.Invoices.Add(invoice);
            }
            else
            {
                // Update existing invoice fields
                _db.Entry(existingInvoice).CurrentValues.SetValues(invoice);

                // Update Client
                if (existingInvoice.Client != null && invoice.Client != null)
                {
                    _db.Entry(existingInvoice.Client).CurrentValues.SetValues(invoice.Client);
                }

                // Handle Invoice Items:
                // Remove deleted items
                foreach (var existingItem in existingInvoice.Items.ToList())
                {
                    if (!invoice.Items.Any(i => i.Id == existingItem.Id))
                    {
                        _db.Remove(existingItem);
                    }
                }

                // Add or update items
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

        public async Task DeleteInvoice(Guid invoiceId)
        {
            var invoice = await _db.Invoices
                .Include(i => i.Items)
                .FirstOrDefaultAsync(i => i.Id == invoiceId);

            if (invoice == null)
                throw new KeyNotFoundException("Invoice not found");

            // Remove related items first (if cascade delete is not configured)
            if (invoice.Items != null)
            {
                _db.Invoice_Items.RemoveRange(invoice.Items);
            }

            _db.Invoices.Remove(invoice);
            await _db.SaveChangesAsync();
        }

        public async Task<string> GetNextInvoiceNumberAsync()
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
    }
}
