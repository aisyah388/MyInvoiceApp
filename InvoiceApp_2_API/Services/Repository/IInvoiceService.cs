using MyInvoiceApp.Shared.Model;
using MyInvoiceApp_Shared.ViewModel;

namespace MyInvoiceApp_API.Services.Repository
{
    public interface IInvoiceService
    {
        Task<List<InvoiceVM>> GetAllInvoicesAsync(Guid companyId);
        Task<Invoice?> GetInvoiceByIdAsync(Guid id, Guid companyId);
        Task<Invoice> CreateInvoiceAsync(Invoice invoice);
        Task<Invoice> UpdateInvoiceAsync(Guid id, Invoice invoice, Guid companyId);
        Task<bool> DeleteInvoiceAsync(Guid id, Guid companyId);
        Task<string> GenerateNextInvoiceNumberAsync(Guid companyId);
        Task<List<InvoiceSummaryVM>> GetInvoiceSummaryAsync(Guid companyId);
    }
}