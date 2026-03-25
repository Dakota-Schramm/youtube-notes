using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using youtube_notes.Models;

namespace youtube_notes.Data
{
    public class youtube_notesContext : IdentityDbContext
    {
        public youtube_notesContext (DbContextOptions<youtube_notesContext> options)
            : base(options)
        {
        }

        public DbSet<youtube_notes.Models.Note> Note { get; set; } = default!;
    }
}
