using TaskBoard.Api.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileProviders;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

builder.Services.AddDbContext<TaskBoardDbContext>(options => options.UseSqlite(builder.Configuration.GetConnectionString("TaskBoardDb")));

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

var app = builder.Build();

string distPath = Path.Combine(Directory.GetCurrentDirectory(), "dist");
if (!Directory.Exists(distPath))
{
    distPath = Path.GetFullPath(Path.Combine(Directory.GetCurrentDirectory(), "../taskboard-ui/dist"));
}
var distProvider = new PhysicalFileProvider(distPath);

app.UseDefaultFiles(new DefaultFilesOptions
{
    FileProvider = distProvider
});

app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = distProvider,
    RequestPath = ""
});

app.MapOpenApi();
var migrateOnStartup = app.Configuration.GetValue<bool>("MigrateOnStartup");
app.Logger.LogInformation("MigrateOnStartup = {Value}", migrateOnStartup);

if (args.Contains("--migrate") || migrateOnStartup)
{
    try
    {
        using var scope = app.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<TaskBoardDbContext>();
        Console.WriteLine("MIGRATE MODE = " + args.Contains("--migrate"));
        Console.WriteLine("CONN = " + app.Configuration.GetConnectionString("TaskBoardDb"));

        db.Database.Migrate();
        Console.WriteLine("MIGRATE SUCCESS");
    }
    catch (Exception ex)
    {
        Console.WriteLine("MIGRATE FAILED: " + ex);
        throw;
    }

}

app.UseHttpsRedirection();
app.UseCors();

app.MapGet("/api/ping", () => Results.Text("pong"));

app.MapGet("/api/tasks", async (TaskBoardDbContext db) =>
    await db.TaskItems
    .Select(t => new TaskResponse(t.Id, t.Title, t.IsComplete))
    .ToListAsync());

app.MapGet("/api/tasks/{id}", async (TaskBoardDbContext db, int id) =>
{
    var task = await db.TaskItems.FindAsync(id);
    if (task == null)
    {
        return Results.NotFound();

    }
    else
    {
        return Results.Ok(new TaskResponse(task.Id, task.Title, task.IsComplete));
    }
});

app.MapPost("/api/tasks", async (TaskBoardDbContext db, CreateTaskRequest createTaskRequest) =>
{
    var title = createTaskRequest.Title;
    if (string.IsNullOrWhiteSpace(title))
    {

        return Results.BadRequest(new { Error = "Title cannot be empty." });
    }
    var newTask = new TaskItem { Title = title };
    db.TaskItems.Add(newTask);
    await db.SaveChangesAsync();
    return Results.Created($"/tasks/{newTask.Id}", new TaskResponse(newTask.Id, newTask.Title, newTask.IsComplete));
});

app.MapPut("/api/tasks/{id}", async (TaskBoardDbContext db, int id, UpdateTaskRequest updateTaskRequest) =>
{
    var task = await db.TaskItems.FindAsync(id);
    if (task == null)
    {
        return Results.NotFound();
    }

    var title = updateTaskRequest.Title;
    if (title == "" || string.IsNullOrWhiteSpace(title))
    {
        return Results.BadRequest(new { Error = "Title cannot be empty." });
    }

    task.Title = title;
    await db.SaveChangesAsync();
    return Results.Ok(new TaskResponse(task.Id, task.Title, task.IsComplete));
});

app.MapDelete("/api/tasks/{id}", async (TaskBoardDbContext db, int id) =>
{
    var task = await db.TaskItems.FindAsync(id);
    if (task == null) return Results.NotFound();
    db.TaskItems.Remove(task);
    await db.SaveChangesAsync();
    return Results.NoContent();
});

app.Run();

public record CreateTaskRequest(string Title);
public record TaskResponse(int Id, string Title, bool IsComplete);

public record UpdateTaskRequest(string Title);

public partial class Program { }
