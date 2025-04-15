using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using Todoapp.Context;
using Todoapp.Entities;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
builder.Services.AddDbContext<ApplicationContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

var summaries = new[]
{
    "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
};

app.MapGet("/weatherforecast", () =>
{
    var forecast =  Enumerable.Range(1, 5).Select(index =>
        new WeatherForecast
        (
            DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
            Random.Shared.Next(-20, 55),
            summaries[Random.Shared.Next(summaries.Length)]
        ))
        .ToArray();
    return forecast;
})
.WithName("GetWeatherForecast");

app.MapGet("/", () => "Hello World!")
    .WithName("GetHelloWorld")
    .WithOpenApi(); 

app.MapPost("/todo", async(Todo todo,ApplicationContext context) =>
{
    // Logic to add a new todo item
    context.Todos.Add(todo);
    await context.SaveChangesAsync();
    return TypedResults.CreatedAtRoute(todo,"GetTodo", new { id = todo.Id });
});

app.MapGet("/todo", async(ApplicationContext context) =>
{
    // Logic to add a new todo item
var todos = await context.Todos.ToListAsync(); 
    return TypedResults.Ok(todos);
});

app.MapPut("/todo/{id:int}",async Task<Results<Ok<Todo>,NotFound>> (int id, Todo todo, ApplicationContext context) =>
{
    

    var existingTodo = await context.Todos.FindAsync(id);
    if (existingTodo is null)
    {
        return TypedResults.NotFound();
    }


    existingTodo.Title = todo.Title;
    existingTodo.Description = todo.Description;
    existingTodo.IsCompleted = todo.IsCompleted;
    existingTodo.UpdatedAt = DateTime.UtcNow;
    await context.SaveChangesAsync();
    return TypedResults.Ok(existingTodo);
});

app.MapGet("/todo/{id:int}", async Task<Results<Ok<Todo>,NotFound>> (int id, ApplicationContext context) =>

{
    var todo = await context.Todos.FirstOrDefaultAsync(t => t.Id == id);
    if (todo is null)
    {
        return TypedResults.NotFound();
    }
    return TypedResults.Ok(todo);
    
}).WithName("GetTodo");

app.MapDelete("/todo/{id:int}", async Task<Results<Ok, NotFound>> (int id, ApplicationContext context) =>
{
    var todo = await context.Todos.FindAsync(id);
    if (todo is null)
    {
        return TypedResults.NotFound();
    }
    context.Todos.Remove(todo);
    await context.SaveChangesAsync();
    return TypedResults.Ok();
});

app.Run();


record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}
