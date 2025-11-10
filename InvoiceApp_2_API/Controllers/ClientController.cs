using MyInvoiceApp_API.Services.Interfaces;
using MyInvoiceApp.Shared.ViewModel;
using Microsoft.AspNetCore.Mvc;

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
    }
}