using MyInvoiceApp_API.Services.Repository;
using MyInvoiceApp.Shared.ViewModel;
using Microsoft.AspNetCore.Mvc;
using MyInvoiceApp.Shared.Model;
using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace MyInvoiceApp_API.Controller
{
    [ApiController]
    [Route("api/client")]
    [Authorize] // ✅ Protect routes
    public class ClientController : ControllerBase
    {
        private readonly IClientService _clientService;

        public ClientController(IClientService clientService)
        {
            _clientService = clientService;
        }

        // Helper method to extract companyId from JWT claims
        private Guid GetCompanyIdFromUser()
        {
            var companyIdClaim = User.FindFirst("CompanyId")?.Value;
            if (string.IsNullOrEmpty(companyIdClaim))
                throw new UnauthorizedAccessException("CompanyId claim missing in token.");

            return Guid.Parse(companyIdClaim);
        }

        [HttpGet("all-clients")]
        [ProducesResponseType(typeof(List<ClientVM>), StatusCodes.Status200OK)]
        public async Task<ActionResult<List<ClientVM>>> GetAllClients()
        {
            var companyId = GetCompanyIdFromUser();
            var clients = await _clientService.GetAllClientsAsync(companyId);
            return Ok(clients);
        }

        [HttpGet("{id}")]
        [ProducesResponseType(typeof(ClientVM), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ClientVM>> GetClientById(Guid id)
        {
            var companyId = GetCompanyIdFromUser();
            var client = await _clientService.GetClientByIdAsync(id, companyId);

            if (client == null)
                return NotFound(new { message = $"Client with ID {id} not found for this company." });

            return Ok(client);
        }

        [HttpPost("add")]
        [ProducesResponseType(typeof(Client), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<Client>> CreateClient([FromBody] Client client)
        {
            var companyId = GetCompanyIdFromUser();

            try
            {
                var createdClient = await _clientService.CreateClientAsync(client, companyId);
                return CreatedAtAction(
                    nameof(GetClientById),
                    new { id = createdClient.Id },
                    createdClient
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
        [ProducesResponseType(typeof(Client), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<Client>> UpdateClient(Guid id, [FromBody] Client client)
        {
            var companyId = GetCompanyIdFromUser();

            try
            {
                var updatedClient = await _clientService.UpdateClientAsync(id, client, companyId);
                return Ok(updatedClient);
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
        public async Task<IActionResult> DeleteClient(Guid id)
        {
            var companyId = GetCompanyIdFromUser();
            var deleted = await _clientService.DeleteClientAsync(id, companyId);

            if (!deleted)
                return NotFound(new { message = $"Client not found for this company." });

            return NoContent();
        }
    }
}
