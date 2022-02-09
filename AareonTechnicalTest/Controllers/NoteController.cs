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
    public class NoteController : ControllerBase
    {
        private readonly ApplicationContext _applicationContext;
        private readonly ILogger<NoteController> _logger;

        public NoteController(ApplicationContext applicationContext, ILogger<NoteController> logger)
        {
            _applicationContext = applicationContext;
            // please note - logging is in console for this quick demo
            _logger = logger;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Note>>> GetNotes()
        {
            _logger.LogInformation($"[NoteController.GetNotes] Returning Notes @{DateTimeOffset.UtcNow}");
            return await _applicationContext.Notes.ToListAsync();
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<Note>> GetNoteById(int id)
        {
            _logger.LogInformation($"[NoteController.GetNoteById] Return Note for id:={id} @{DateTimeOffset.UtcNow}");
            var note = await _applicationContext.Notes.FindAsync(id);
            if (note == null)
            {
                _logger.LogError($"[NoteController.GetNoteById] No Note found for id:={id} @{DateTimeOffset.UtcNow}");
                return NotFound();
            }
            return note;
        }


        [HttpPost]
        public async Task<ActionResult<Note>> PostNote(Note note)
        {
            _logger.LogInformation($"[NoteController.PostNote] Saving new Note @{DateTimeOffset.UtcNow}");
            if (!IsValidPerson(note.PersonId))
            {
                return BadRequest("The personId is invalid.");
            }
            if (note.TicketId.HasValue && !IsValidTicket(note.TicketId.Value))
            {
                return BadRequest("The ticketId is invalid.");
            }
            _applicationContext.Notes.Add(note);
            await _applicationContext.SaveChangesAsync();
            var result = CreatedAtAction("GetNoteById", new { id = note.Id }, note);
            _logger.LogInformation($"[NoteController.PostNote] Saved new note with id:={note.Id} @{DateTimeOffset.UtcNow}");
            return result;
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> PutNote(int id, Note note)
        {
            _logger.LogInformation($"[NoteController.PutNote] Updating Note for id:={id} @{DateTimeOffset.UtcNow}");
            var noteToUpdate = await _applicationContext.Notes.FindAsync(id);
            if (noteToUpdate == null)
            {
                _logger.LogError($"[NoteController.PutNote] No Note found for id:={id} @{DateTimeOffset.UtcNow}");
                return NotFound();
            }
            if (!IsValidPerson(note.PersonId))
            {
                return BadRequest("The personId is invalid.");
            }
            if (note.TicketId.HasValue && !IsValidTicket(note.TicketId.Value))
            {
                return BadRequest("The ticketId is invalid.");
            }
            noteToUpdate.Update(note);
            _applicationContext.Entry(noteToUpdate).State = EntityState.Modified;
            try
            {
                await _applicationContext.SaveChangesAsync();
                _logger.LogInformation($"[NoteController.PutNote] Updated Note for id:={id} @{DateTimeOffset.UtcNow}");
            }
            catch (DbUpdateConcurrencyException ex)
            {
                _logger.LogError($"[NoteController.PutNote] Error while updating Note found for id:={id} @{DateTimeOffset.UtcNow}. Error:= {ex}");
                if (!NoteExists(id))
                {
                    return NotFound();
                }
                throw;
            }
            return NoContent();
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> DeleteNoteById(int id, int operatorId)
        {
            var person = await _applicationContext.Persons.FindAsync(operatorId);
            if (person == null || !person.IsAdmin)
            {
                return Unauthorized("The operator is unauthorised");
            }
            _logger.LogInformation($"[NoteController.DeleteNoteById] Deleting Note with id:={id} @{DateTimeOffset.UtcNow}");
            var note = await _applicationContext.Notes.FindAsync(id);
            if (note == null)
            {
                _logger.LogError($"[NoteController.DeleteNoteById] No Note found for id:={id} @{DateTimeOffset.UtcNow}");
                return NotFound();
            }
            _applicationContext.Notes.Remove(note);
            await _applicationContext.SaveChangesAsync();
            _logger.LogInformation($"[NoteController.DeleteNoteById] Deleted Note with id:={id} @{DateTimeOffset.UtcNow}");
            return NoContent();
        }

        private bool IsValidPerson(int personId)
        {
            return _applicationContext.Persons.Any(e => e.Id == personId);
        }

        private bool IsValidTicket(int ticketId)
        {
            return _applicationContext.Tickets.Any(e => e.Id == ticketId);
        }
        private bool NoteExists(int id)
        {
            return _applicationContext.Notes.Any(e => e.Id == id);
        }

    }
}
