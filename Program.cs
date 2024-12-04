using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.Extensions.DependencyInjection;
using mojefilmy_softwarestudio_be.Models;
using mojefilmy_softwarestudio_be.Properties;
using Npgsql;

#region Builder, Connection string

var builder = WebApplication.CreateBuilder(args);
var connectionStringBuilder = new NpgsqlConnectionStringBuilder();
string dbUrlEnv = Resources.DATABASE_URL;


if (dbUrlEnv == null)
{
  throw new Exception("DATABASE_URL environment variable is not set");
}

Uri databaseUrl = new Uri(dbUrlEnv);

connectionStringBuilder.SslMode = SslMode.Require;
connectionStringBuilder.Host = databaseUrl.Host;
connectionStringBuilder.Port = databaseUrl.Port;
connectionStringBuilder.Username = databaseUrl.UserInfo.Split(':')[0];
connectionStringBuilder.Password = databaseUrl.UserInfo.Split(':')[1];

connectionStringBuilder.Database = "movies";

#endregion

#region Services

builder.Services.AddControllers()
  .AddJsonOptions(options =>
  {
    options.JsonSerializerOptions.PropertyNameCaseInsensitive = true;
  });

builder.Services.AddHttpClient();
builder.Services.AddDbContext<MovieContext>(options =>
    options.UseNpgsql(connectionStringBuilder.ConnectionString));
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddCors(options =>
{
  options.AddPolicy("AllowAll",
      builder => builder
          .AllowAnyOrigin()
          .AllowAnyMethod()
          .AllowAnyHeader());
});

var app = builder.Build();

#endregion

#region Development

if (app.Environment.IsDevelopment())
{
  app.UseHsts();
  app.UseSwagger();
  app.UseSwaggerUI();
}

app.UseCors("AllowAll");

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

#endregion

app.Run();
