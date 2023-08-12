using Microsoft.EntityFrameworkCore;
using Notes.Domain;
using System.Threading.Tasks;
using System.Threading;

namespace Notes.Application.Interfaces
{
    public interface INotesDbContext
    {
        public DbSet<Note> Notes { get; set; }

        Task<int> SaveChangesAsync(CancellationToken cancellationToken);
    }
}
