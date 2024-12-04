using Microsoft.EntityFrameworkCore;

namespace mojefilmy_softwarestudio_be.Models
{
  public class MovieContext : DbContext
  {
    public MovieContext(DbContextOptions<MovieContext> options)
      : base(options)
    {
      //AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);
    }
    public DbSet<Movie> Movies { get; set; } = null!;
  }
}
