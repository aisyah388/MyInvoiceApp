using MyInvoiceApp.Shared.Model;
using MyInvoiceApp_Shared.ViewModel;

namespace MyInvoiceApp_API.Services.Repository
{
    public interface IInvoiceService
    {
        Task<List<InvoiceVM>> GetAllInvoicesAsync();
        Task<Invoice?> GetInvoiceByIdAsync(Guid id);
        Task<Invoice> CreateInvoiceAsync(Invoice invoice);
        Task<Invoice> UpdateInvoiceAsync(Guid id, Invoice invoice);
        Task<bool> DeleteInvoiceAsync(Guid id);
        Task<string> GenerateNextInvoiceNumberAsync();
        Task<List<InvoiceSummaryVM>> GetInvoiceSummaryAsync();
    }
}