using MyInvoiceApp_API.Services.Repository;
using MyInvoiceApp.Shared.Model;
using MyInvoiceApp_Shared.ViewModel;
using Microsoft.AspNetCore.Mvc;
using FluentValidation;

namespace MyInvoiceApp_API.Controller
{
    [ApiController]
    [Route("api/invoice")]
    public class InvoiceController : ControllerBase
    {
        private readonly IInvoiceService _invoiceService;

        public InvoiceController(IInvoiceService invoiceService)
        {
            _invoiceService = invoiceService;
        }

        [HttpGet("all-invoices")]
        [ProducesResponseType(typeof(List<InvoiceVM>), StatusCodes.Status200OK)]
        public async Task<ActionResult<List<InvoiceVM>>> GetAllInvoices()
        {
            var invoices = await _invoiceService.GetAllInvoicesAsync();
            return Ok(invoices);
        }

        [HttpGet("{id}")]
        [ProducesResponseType(typeof(Invoice), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<Invoice>> GetInvoiceById(Guid id)
        {
            var invoice = await _invoiceService.GetInvoiceByIdAsync(id);

            if (invoice == null)
            {
                return NotFound(new { message = $"Invoice with ID {id} not found." });
            }

            return Ok(invoice);
        }

        [HttpPost("add")]
        [ProducesResponseType(typeof(Invoice), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<Invoice>> CreateInvoice([FromBody] Invoice invoice)
        {
            try
            {
                var createdInvoice = await _invoiceService.CreateInvoiceAsync(invoice);
                return CreatedAtAction(
                    nameof(GetInvoiceById),
                    new { id = createdInvoice.Id },
                    createdInvoice
                );
            }
            catch (ValidationException ex)
            {
                return BadRequest(new
                {
                    message = "Validation failed",
                    errors = ex.Errors.Select(e => new
                    {
                        property = e.PropertyName,
                        error = e.ErrorMessage
                    })
                });
            }
        }

        [HttpPut("{id}")]
        [ProducesResponseType(typeof(Invoice), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<Invoice>> UpdateInvoice(Guid id, [FromBody] Invoice invoice)
        {
            try
            {
                var updatedInvoice = await _invoiceService.UpdateInvoiceAsync(id, invoice);
                return Ok(updatedInvoice);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (ValidationException ex)
            {
                return BadRequest(new
                {
                    message = "Validation failed",
                    errors = ex.Errors.Select(e => new
                    {
                        property = e.PropertyName,
                        error = e.ErrorMessage
                    })
                });
            }
        }

        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteInvoice(Guid id)
        {
            var deleted = await _invoiceService.DeleteInvoiceAsync(id);

            if (!deleted)
            {
                return NotFound(new { message = $"Invoice with ID {id} not found." });
            }

            return NoContent();
        }

        [HttpGet("next-inv-number")]
        [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
        public async Task<ActionResult<string>> GetNextInvoiceNumber()
        {
            var number = await _invoiceService.GenerateNextInvoiceNumberAsync();
            return Ok(number);
        }

        [HttpGet("summary")]
        [ProducesResponseType(typeof(List<InvoiceSummaryVM>), StatusCodes.Status200OK)]
        public async Task<ActionResult<List<InvoiceSummaryVM>>> GetInvoiceSummary()
        {
            var summary = await _invoiceService.GetInvoiceSummaryAsync();
            return Ok(summary);
        }
    }
}