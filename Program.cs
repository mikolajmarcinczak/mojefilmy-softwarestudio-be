using Microsoft.EntityFrameworkCore;
using mojefilmy_softwarestudio_be.Models;
using Npgsql;

#region Builder, Connection string

var builder = WebApplication.CreateBuilder(args);
var connectionStringBuilder = new NpgsqlConnectionStringBuilder();
connectionStringBuilder.SslMode = SslMode.VerifyFull;
string dbUrlEnv = Environment.GetEnvironmentVariable("DATABASE_URL");

if (dbUrlEnv != null)
{
  throw new Exception("DATABASE_URL environment variable is not set");
}

Uri databaseUrl = new Uri(dbUrlEnv);
connectionStringBuilder.Host = databaseUrl.Host;
connectionStringBuilder.Port = databaseUrl.Port;
connectionStringBuilder.Username = databaseUrl.UserInfo.Split(':')[0];
connectionStringBuilder.Password = databaseUrl.UserInfo.Split(':')[1];

connectionStringBuilder.Database = "movies";

#endregion

#region Services

builder.Services.AddControllers();
builder.Services.AddHttpClient();
builder.Services.AddDbContextPool<MovieContext>(options =>
    options.UseNpgsql(connectionStringBuilder.ConnectionString));
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


var app = builder.Build();

#endregion

#region Development

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

#endregion

app.Run();
