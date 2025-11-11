using MyInvoiceApp_API.Services.Repository;
using MyInvoiceApp.Shared.ViewModel;
using MyInvoiceApp_Shared.ViewModel;
using Microsoft.AspNetCore.Mvc;

namespace MyInvoiceApp_API.Controller
{
    [ApiController]
    [Route("api/status")]
    public class StatusController : ControllerBase
    {
        private readonly IStatusService _statusService;

        public StatusController(IStatusService statusService)
        {
            _statusService = statusService;
        }

        [HttpGet("all-statuses")]
        [ProducesResponseType(typeof(List<StatusVM>), StatusCodes.Status200OK)]
        public async Task<ActionResult<List<StatusVM>>> GetAllStatuses()
        {
            var statuses = await _statusService.GetAllStatusesAsync();
            return Ok(statuses);
        }

        [HttpGet("invoice-count")]
        [ProducesResponseType(typeof(List<StatusSummaryVM>), StatusCodes.Status200OK)]
        public async Task<ActionResult<List<StatusSummaryVM>>> GetInvoiceCountByStatus()
        {
            var summary = await _statusService.GetInvoiceCountByStatusAsync();
            return Ok(summary);
        }
    }
}