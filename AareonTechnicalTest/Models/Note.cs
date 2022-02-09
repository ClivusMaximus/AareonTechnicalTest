using System.ComponentModel.DataAnnotations;

namespace AareonTechnicalTest.Models
{
    public class Note
    {
        [Key]
        public int Id { get; }

        public int PersonId { get; set; }

        public int? TicketId { get; set; }

        public string Detail { get; set; }

        public void Update(Note note)
        {
            if (note != null)
            {
                PersonId = note.PersonId;
                TicketId = note.TicketId;
                Detail = note.Detail;
            }
        }
    }
}
