namespace Notes.Persistence
{
    public class DbInitializer
    {
        public static void Initialize(NotesDbContext context)
        {
            //Если база не создана, она создаётся :))
            context.Database.EnsureCreated();
        }
    }
}
