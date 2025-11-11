using MyInvoiceApp_API.Services.Repository;
using MyInvoiceApp.Shared.ViewModel;
using Microsoft.AspNetCore.Mvc;
using MyInvoiceApp.Shared.Model;
using FluentValidation;

namespace MyInvoiceApp_API.Controller
{
    [ApiController]
    [Route("api/client")]
    public class ClientController : ControllerBase
    {
        private readonly IClientService _clientService;

        public ClientController(IClientService clientService)
        {
            _clientService = clientService;
        }

        [HttpGet("all-clients")]
        [ProducesResponseType(typeof(List<ClientVM>), StatusCodes.Status200OK)]
        public async Task<ActionResult<List<ClientVM>>> GetAllClients()
        {
            var clients = await _clientService.GetAllClientsAsync();
            return Ok(clients);
        }

        [HttpGet("{id}")]
        [ProducesResponseType(typeof(ClientVM), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ClientVM>> GetClientById(Guid id)
        {
            var client = await _clientService.GetClientByIdAsync(id);

            if (client == null)
            {
                return NotFound(new { message = $"Client with ID {id} not found." });
            }

            return Ok(client);
        }

        [HttpPost("add")]
        [ProducesResponseType(typeof(Client), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<Client>> CreateClient([FromBody] Client client)
        {
            try
            {
                var createdInvoice = await _clientService.CreateClientAsync(client);
                return CreatedAtAction(
                    nameof(GetClientById),
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
        [ProducesResponseType(typeof(Client), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<Client>> UpdateInvoice(Guid id, [FromBody] Client client)
        {
            try
            {
                var updatedInvoice = await _clientService.UpdateClientAsync(id, client);
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
        public async Task<IActionResult> DeleteClient(Guid id)
        {
            var deleted = await _clientService.DeleteClientAsync(id);

            if (!deleted)
            {
                return NotFound(new { message = $"Client not found." });
            }

            return NoContent();
        }
    }
}