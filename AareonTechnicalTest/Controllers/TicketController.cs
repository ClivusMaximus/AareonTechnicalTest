using System;
using AareonTechnicalTest.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace AareonTechnicalTest.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TicketController : ControllerBase
    {
        private readonly ApplicationContext _applicationContext;
        private readonly ILogger<TicketController> _logger;

        public TicketController(ApplicationContext applicationContext, ILogger<TicketController> logger)
        {
            _applicationContext = applicationContext;
            // please note - logging is in console for this quick demo
            _logger = logger;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Ticket>>> GetTickets()
        {
            _logger.LogInformation($"[TicketsController.GetTickets] Returning all tickets @{DateTimeOffset.UtcNow}");
            return await _applicationContext.Tickets.ToListAsync();
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<Ticket>> GetTicketById(int id)
        {
            _logger.LogInformation($"[TicketsController.GetTicketById] Return ticket for id:={id} @{DateTimeOffset.UtcNow}");
            var ticket = await _applicationContext.Tickets.FindAsync(id);
            if (ticket == null)
            {
                _logger.LogError($"[TicketsController.GetTicketById] No ticket found for id:={id} @{DateTimeOffset.UtcNow}");
                return NotFound();
            }
            return ticket;
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> PutTicket(int id, Ticket ticket)
        {
            _logger.LogInformation($"[TicketsController.PutTicket] Updating ticket for id:={id} @{DateTimeOffset.UtcNow}");
            var ticketToUpdate = await _applicationContext.Tickets.FindAsync(id);
            if (ticketToUpdate == null)
            {
                _logger.LogError($"[TicketsController.PutTicket] No ticket found for id:={id} @{DateTimeOffset.UtcNow}");
                return NotFound();
            }
            if (!IsValidPerson(ticket.PersonId))
            {
                return BadRequest("Invalid PersonId");
            }
            ticketToUpdate.Update(ticket);
            _applicationContext.Entry(ticketToUpdate).State = EntityState.Modified;
            try
            {
                await _applicationContext.SaveChangesAsync();
                _logger.LogInformation($"[TicketsController.PutTicket] Updated ticket for id:={id} @{DateTimeOffset.UtcNow}");
            }
            catch (DbUpdateConcurrencyException ex)
            {
                _logger.LogError($"[TicketsController.PutTicket] Error while updating ticket found for id:={id} @{DateTimeOffset.UtcNow}. Error:= {ex}");
                if (!TicketExists(id))
                {
                    return NotFound();
                }
                throw;
            }
            return NoContent();
        }

        [HttpPost]
        public async Task<ActionResult<Ticket>> PostTicket(Ticket ticket)
        {
            _logger.LogInformation($"[TicketsController.PostTicket] Saving new ticket @{DateTimeOffset.UtcNow}");
            if (!IsValidPerson(ticket.PersonId))
            {
                return BadRequest("Invalid PersonId");
            }
            _applicationContext.Tickets.Add(ticket);
            await _applicationContext.SaveChangesAsync();
            var result = CreatedAtAction("GetTicketById", new { id = ticket.Id }, ticket);
            _logger.LogInformation($"[TicketsController.PostTicket] Saved new ticket with id:={ticket.Id} @{DateTimeOffset.UtcNow}");
            return result;
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> DeleteTicketById(int id)
        {
            _logger.LogInformation($"[TicketsController.DeleteTicketById] Deleting ticket with id:={id} @{DateTimeOffset.UtcNow}");
            var ticket = await _applicationContext.Tickets.FindAsync(id);
            if (ticket == null)
            {
                _logger.LogError($"[TicketsController.DeleteTicketById] No ticket found for id:={id} @{DateTimeOffset.UtcNow}");
                return NotFound();
            }
            _applicationContext.Tickets.Remove(ticket);
            await _applicationContext.SaveChangesAsync();
            _logger.LogInformation($"[TicketsController.DeleteTicketById] Deleted ticket with id:={id} @{DateTimeOffset.UtcNow}");
            return NoContent();
        }

        private bool TicketExists(int id)
        {
            return _applicationContext.Tickets.Any(e => e.Id == id);
        }

        private bool IsValidPerson(int personId)
        {
            return _applicationContext.Persons.Any(e => e.Id == personId);
        }

    }
}