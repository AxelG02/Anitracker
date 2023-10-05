using Microsoft.EntityFrameworkCore;

namespace anitracker_minimal_api
{
    public class TitleDb : DbContext
    {
        public TitleDb(DbContextOptions<TitleDb> options) : base(options) { }
        public DbSet<TitleItem> Titles => Set<TitleItem>();
    }
}
