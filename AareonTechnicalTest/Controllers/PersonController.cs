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
    public class PersonController : ControllerBase
    {
        private readonly ApplicationContext _applicationContext;
        private readonly ILogger<PersonController> _logger;

        public PersonController(ApplicationContext applicationContext, ILogger<PersonController> logger)
        {
            _applicationContext = applicationContext;
            // please note - logging is in console for this quick demo
            _logger = logger;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Person>>> GetPersons()
        {
            _logger.LogInformation($"[PersonController.GetPersons] Returning Persons @{DateTimeOffset.UtcNow}");
            return await _applicationContext.Persons.ToListAsync();
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<Person>> GetPersonById(int id)
        {
            _logger.LogInformation($"[PersonController.GetPersonById] Return Person for id:={id} @{DateTimeOffset.UtcNow}");
            var person = await _applicationContext.Persons.FindAsync(id);
            if (person == null)
            {
                _logger.LogError($"[PersonController.GetPersonById] No Person found for id:={id} @{DateTimeOffset.UtcNow}");
                return NotFound();
            }
            return person;
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> PutPerson(int id, Person person)
        {
            _logger.LogInformation($"[PersonController.PutPerson] Updating Person for id:={id} @{DateTimeOffset.UtcNow}");
            var personToUpdate = await _applicationContext.Persons.FindAsync(id);
            if (personToUpdate == null)
            {
                _logger.LogError($"[PersonController.PutPerson] No Person found for id:={id} @{DateTimeOffset.UtcNow}");
                return NotFound();
            }
            personToUpdate.Update(person);
            _applicationContext.Entry(personToUpdate).State = EntityState.Modified;
            try
            {
                await _applicationContext.SaveChangesAsync();
                _logger.LogInformation($"[PersonController.PutPerson] Updated Person for id:={id} @{DateTimeOffset.UtcNow}");
            }
            catch (DbUpdateConcurrencyException ex)
            {
                _logger.LogError($"[PersonController.PutPerson] Error while updating Person found for id:={id} @{DateTimeOffset.UtcNow}. Error:= {ex}");
                if (!PersonExists(id))
                {
                    return NotFound();
                }
                throw;
            }
            return NoContent();
        }

        [HttpPost]
        public async Task<ActionResult<Ticket>> PostPerson(Person person)
        {
            _logger.LogInformation($"[PersonController.PostPerson] Saving new Person @{DateTimeOffset.UtcNow}");
            _applicationContext.Persons.Add(person);
            await _applicationContext.SaveChangesAsync();
            var result = CreatedAtAction("GetPersonById", new { id = person.Id }, person);
            _logger.LogInformation($"[PersonController.PostPerson] Saved new person with id:={person.Id} @{DateTimeOffset.UtcNow}");
            return result;
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> DeletePersonById(int id)
        {
            _logger.LogInformation($"[PersonController.DeletePersonById] Deleting Person with id:={id} @{DateTimeOffset.UtcNow}");
            var ticket = await _applicationContext.Tickets.FindAsync(id);
            if (ticket == null)
            {
                _logger.LogError($"[PersonController.DeletePersonById] No Person found for id:={id} @{DateTimeOffset.UtcNow}");
                return NotFound();
            }
            _applicationContext.Tickets.Remove(ticket);
            await _applicationContext.SaveChangesAsync();
            _logger.LogInformation($"[PersonController.DeletePersonById] Deleted Person with id:={id} @{DateTimeOffset.UtcNow}");
            return NoContent();
        }

        private bool PersonExists(int id)
        {
            return _applicationContext.Persons.Any(e => e.Id == id);
        }

    }
}