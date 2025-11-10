using MyInvoiceApp.Shared.ViewModel;
using MyInvoiceApp_Shared.ViewModel;

namespace MyInvoiceApp_API.Services.Interfaces
{
    public interface IStatusService
    {
        Task<List<StatusVM>> GetAllStatusesAsync();
        Task<List<StatusSummaryVM>> GetInvoiceCountByStatusAsync();
    }
}