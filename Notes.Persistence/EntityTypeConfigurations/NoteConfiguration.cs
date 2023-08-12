using Microsoft.EntityFrameworkCore;
using Notes.Domain;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Notes.Persistence.EntityTypeConfigurations
{
    /// <summary>
    /// FluentAPI для настройки БД
    /// </summary>
    public class NoteConfiguration : IEntityTypeConfiguration<Note>
    {
        public void Configure(EntityTypeBuilder<Note> builder)
        {
            builder.HasKey(note  => note.Id);
            builder.HasIndex(name => name.Id).IsUnique();
            builder.Property(note => note.Title).HasMaxLength(250);
        }
    }
}
