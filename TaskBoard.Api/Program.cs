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

app.MapGet("/tasks", (TaskBoardDbContext db) => db.TaskItems.ToList());

app.MapPost("/tasks", async (TaskBoardDbContext db, HttpRequest request) =>
{
    using var reader = new StreamReader(request.Body);
    var title = await reader.ReadToEndAsync();
   var newTask = new TaskItem { Title = title };
   db.TaskItems.Add(newTask);
   await db.SaveChangesAsync();
   return Results.Created("/tasks", newTask);
});

app.Run();

public partial class Program { }
