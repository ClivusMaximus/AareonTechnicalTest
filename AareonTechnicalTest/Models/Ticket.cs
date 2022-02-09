using System;
using System.ComponentModel.DataAnnotations;

namespace AareonTechnicalTest.Models
{
    public class Ticket
    {
        [Key]
        public int Id { get; }

        public string Content { get; set; }

        public int PersonId { get; set; }

        internal void Update(Ticket ticket)
        {
            if (ticket == null)
            {
                return;
            }
            Content = ticket.Content;
            PersonId = ticket.PersonId;
        }
    }
}