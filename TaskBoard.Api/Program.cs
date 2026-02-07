using TaskBoard.Api.Data;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

builder.Services.AddDbContext<TaskBoardDbContext>(options => options.UseSqlite(builder.Configuration.GetConnectionString("TaskBoardDb")));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    using var scope = app.Services.CreateScope();
    var dbContext = scope.ServiceProvider.GetRequiredService<TaskBoardDbContext>();
    dbContext.Database.Migrate();
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.MapGet("/ping", () => Results.Text("pong"));

app.MapGet("/tasks", async (TaskBoardDbContext db) =>
    await db.TaskItems
    .Select(t => new TaskResponse(t.Id, t.Title, t.IsComplete))
    .ToListAsync());

app.MapGet("/tasks/{id}", async (TaskBoardDbContext db, int id) =>
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

app.MapPost("/tasks", async (TaskBoardDbContext db, CreateTaskRequest createTaskRequest) =>
{
    var title = createTaskRequest.Title;
    if (title == "" || string.IsNullOrWhiteSpace(title))
    {
        return Results.BadRequest();
    }
    var newTask = new TaskItem { Title = title };
    db.TaskItems.Add(newTask);
    await db.SaveChangesAsync();
    return Results.Created("/tasks", new TaskResponse(newTask.Id, newTask.Title, newTask.IsComplete));
});

app.MapPut("/tasks/{id}", async (TaskBoardDbContext db, int id, CreateTaskRequest updateTaskRequest) =>
{
    var task = await db.TaskItems.FindAsync(id);
    if (task == null)
    {
        return Results.NotFound();
    }

    var title = updateTaskRequest.Title;
    if (title == "" || string.IsNullOrWhiteSpace(title))
    {
        return Results.BadRequest();
    }

    task.Title = title;
    await db.SaveChangesAsync();
    return Results.Ok(new TaskResponse(task.Id, task.Title, task.IsComplete));
});

app.MapDelete("/tasks/{id}", async (TaskBoardDbContext db, int id) =>
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

public partial class Program { }
