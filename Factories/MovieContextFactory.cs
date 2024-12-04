using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using mojefilmy_softwarestudio_be.Models;
using mojefilmy_softwarestudio_be.Properties;
using Npgsql;

namespace mojefilmy_softwarestudio_be.Factories
{
  public class MovieContextFactory : IDesignTimeDbContextFactory<MovieContext>
  {
    public MovieContext CreateDbContext(string[] args)
    {
      var optionsBuilder = new DbContextOptionsBuilder<MovieContext>();
      var connectionStringBuilder = new NpgsqlConnectionStringBuilder();
      string dbUrlEnv = Resources.DATABASE_URL;

      if (string.IsNullOrEmpty(dbUrlEnv))
      {
        throw new System.Exception("DATABASE_URL variable is not set");
      }

      Uri databaseUrl = new Uri(dbUrlEnv);

      connectionStringBuilder.SslMode = SslMode.Require;
      connectionStringBuilder.Host = databaseUrl.Host;
      connectionStringBuilder.Port = databaseUrl.Port;
      connectionStringBuilder.Username = databaseUrl.UserInfo.Split(':')[0];
      connectionStringBuilder.Password = databaseUrl.UserInfo.Split(':')[1];
      connectionStringBuilder.Database = "movies";

      optionsBuilder.UseNpgsql(connectionStringBuilder.ConnectionString);

      return new MovieContext(optionsBuilder.Options);
    }
  }
}
