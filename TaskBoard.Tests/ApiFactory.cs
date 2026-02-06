using System.Data.Common;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using TaskBoard.Api.Data;

namespace TaskBoard.Tests;

public class ApiFactory : WebApplicationFactory<Program>
{
    private DbConnection? _connection;

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Testing");
        builder.ConfigureServices(services =>
        {
            // Remove the app's DbContext registration
            var descriptor = services.Single(d =>
                d.ServiceType == typeof(DbContextOptions<TaskBoardDbContext>));
            services.Remove(descriptor);

            // One in-memory SQLite connection shared for the host lifetime
            _connection ??= new SqliteConnection("DataSource=:memory:");
            if (_connection.State != System.Data.ConnectionState.Open)
                _connection.Open();

            services.AddDbContext<TaskBoardDbContext>(options =>
                options.UseSqlite(_connection));

            // Rebuild provider and reset schema deterministically
            var sp = services.BuildServiceProvider();
            using var scope = sp.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<TaskBoardDbContext>();

            db.Database.EnsureDeleted();
            db.Database.Migrate();
        });
    }

    protected override void Dispose(bool disposing)
    {
        base.Dispose(disposing);
        if (disposing) _connection?.Dispose();
    }
}
