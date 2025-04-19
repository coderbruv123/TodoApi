using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using Todoapp.Context;
using Todoapp.Entities;
using Todoapp.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
builder.Services.AddDbContext<ApplicationContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
builder.Services.AddScoped<ITodoServices, TodoService>();


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();




app.MapGet("/", () => "Hello World!")
    .WithName("GetHelloWorld")
    .WithOpenApi(); 
    

app.MapPost("/todo", async(ITodoServices todoServices,Todo todos) =>
{
    var todo = await todoServices!.AddTodoAsync(todos);
    return TypedResults.CreatedAtRoute(todo);
});

app.MapGet("/todo", async(ITodoServices todoservice) =>
{
    var todos =  await todoservice.GetTodosAsync();
    return TypedResults.Ok(todos);
});


app.MapPut("/todo/{id:int}",async Task<Results<Ok<Todo>,NotFound>> (int id, Todo todo, ITodoServices todoService) =>
{

    var existingTodo = await todoService.UpdateTodoAsync(id,todo);
    if (existingTodo is null)
    {
        return TypedResults.NotFound();
    }
    return TypedResults.Ok(existingTodo);
});

app.MapGet("/todo/{id:int}", async Task<Results<Ok<Todo>,NotFound>> (int id, ITodoServices todoServices) =>

{
    try{
    var todo = await todoServices.GetTodoAsync(id);
    if (todo is null)
    {
        return TypedResults.NotFound();
    }
    return TypedResults.Ok(todo);
    }
    catch (Exception ex)
    {
        Console.WriteLine($"An error occurred: {ex.Message}");
        return TypedResults.NotFound();
       
    }
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


