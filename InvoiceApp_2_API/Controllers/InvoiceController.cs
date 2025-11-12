using MyInvoiceApp_API.Services.Repository;
using MyInvoiceApp.Shared.Model;
using MyInvoiceApp_Shared.ViewModel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using FluentValidation;
using System.Security.Claims;

namespace MyInvoiceApp_API.Controller
{
    [Authorize] // Add authorization to entire controller
    [ApiController]
    [Route("api/invoice")]
    public class InvoiceController : ControllerBase
    {
        private readonly IInvoiceService _invoiceService;

        public InvoiceController(IInvoiceService invoiceService)
        {
            _invoiceService = invoiceService;
        }

        // Helper method to extract CompanyId from JWT
        private Guid GetCompanyId()
        {
            var companyIdClaim = User.FindFirst("CompanyId")?.Value;
            if (string.IsNullOrEmpty(companyIdClaim) || !Guid.TryParse(companyIdClaim, out var companyId))
            {
                throw new UnauthorizedAccessException("Company ID not found in token");
            }
            return companyId;
        }

        [HttpGet("all-invoices")]
        [ProducesResponseType(typeof(List<InvoiceVM>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<List<InvoiceVM>>> GetAllInvoices()
        {
            try
            {
                var companyId = GetCompanyId();
                var invoices = await _invoiceService.GetAllInvoicesAsync(companyId);
                return Ok(invoices);
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(new { message = ex.Message });
            }
        }

        [HttpGet("{id}")]
        [ProducesResponseType(typeof(Invoice), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<Invoice>> GetInvoiceById(Guid id)
        {
            try
            {
                var companyId = GetCompanyId();
                var invoice = await _invoiceService.GetInvoiceByIdAsync(id, companyId);

                if (invoice == null)
                {
                    return NotFound(new { message = $"Invoice with ID {id} not found." });
                }

                return Ok(invoice);
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(new { message = ex.Message });
            }
        }

        [HttpPost("add")]
        [ProducesResponseType(typeof(Invoice), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<Invoice>> CreateInvoice([FromBody] Invoice invoice)
        {
            try
            {
                var companyId = GetCompanyId();

                // Set the company ID from the token
                invoice.Company_Id = companyId;

                var createdInvoice = await _invoiceService.CreateInvoiceAsync(invoice);
                return CreatedAtAction(
                    nameof(GetInvoiceById),
                    new { id = createdInvoice.Id },
                    createdInvoice
                );
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(new { message = ex.Message });
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
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<Invoice>> UpdateInvoice(Guid id, [FromBody] Invoice invoice)
        {
            try
            {
                var companyId = GetCompanyId();

                // Ensure the company ID matches the token
                invoice.Company_Id = companyId;

                var updatedInvoice = await _invoiceService.UpdateInvoiceAsync(id, invoice, companyId);
                return Ok(updatedInvoice);
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(new { message = ex.Message });
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
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> DeleteInvoice(Guid id)
        {
            try
            {
                var companyId = GetCompanyId();
                var deleted = await _invoiceService.DeleteInvoiceAsync(id, companyId);

                if (!deleted)
                {
                    return NotFound(new { message = $"Invoice with ID {id} not found." });
                }

                return NoContent();
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(new { message = ex.Message });
            }
        }

        [HttpGet("next-inv-number")]
        [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<string>> GetNextInvoiceNumber()
        {
            try
            {
                var companyId = GetCompanyId();
                var number = await _invoiceService.GenerateNextInvoiceNumberAsync(companyId);
                return Ok(number);
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(new { message = ex.Message });
            }
        }

        [HttpGet("summary")]
        [ProducesResponseType(typeof(List<InvoiceSummaryVM>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<List<InvoiceSummaryVM>>> GetInvoiceSummary()
        {
            try
            {
                var companyId = GetCompanyId();
                var summary = await _invoiceService.GetInvoiceSummaryAsync(companyId);
                return Ok(summary);
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(new { message = ex.Message });
            }
        }
    }
}