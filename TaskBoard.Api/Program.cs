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

var tasks = new List<string>
{
    "Task 1",
    "Task 2",
    "Task 3"
};

app.MapGet("/ping", () => Results.Text("pong"));

app.MapGet("/tasks", () => tasks);

app.MapPost("/tasks", async (HttpRequest request) =>
{
   tasks.Add("New Task");
   return Results.Created("/tasks", tasks);
});

app.Run();

public partial class Program { }
