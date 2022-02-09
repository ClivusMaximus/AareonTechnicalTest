using System.ComponentModel.DataAnnotations;

namespace AareonTechnicalTest.Models
{
    public class Person
    {
        [Key]
        public int Id { get; }

        public string Forename { get; set; }

        public string Surname { get; set; }

        public bool IsAdmin { get; set; }

        public void Update(Person person)
        {
            if (person != null)
            {
                Forename = person.Forename;
                Surname = person.Surname;
                IsAdmin = person.IsAdmin;
            }
        }
    }
}
