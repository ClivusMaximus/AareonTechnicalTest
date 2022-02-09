using Microsoft.EntityFrameworkCore;

namespace AareonTechnicalTest.Models
{
    public class NoteConfig
    {
        public static void Configure(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Note>(
                entity =>
                {
                    entity.HasKey(e => e.Id);
                });
        }
    }
}
