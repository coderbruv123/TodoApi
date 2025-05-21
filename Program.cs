using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Todoapp.Context;
using Todoapp.Entities;
using Todoapp.Services;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
builder.Services.AddDbContext<ApplicationContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"])),
            ValidateIssuerSigningKey = true,
        };
    });
builder.Services.AddAuthorization();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        policy.WithOrigins("http://localhost:5173")
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials();
    });
});

builder.Services.AddScoped<ITodoServices, TodoService>();
builder.Services.AddScoped<IAuthServices, AuthServices>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseCors("AllowFrontend");

app.UseAuthentication();
app.UseAuthorization();

app.MapGet("/", () => "Hello World!")
    .WithName("GetHelloWorld")
    .WithOpenApi();

app.MapPost("/register", async Task<IResult>(UserDTO request, IAuthServices services) =>
{
    var user = await services.RegisterAsync(request);
    if (user == null)
    {
        return Results.BadRequest("User already exists");
    }
    return Results.Created($"/users/{user.Id}", new { user.Id, user.Username });
});

app.MapPost("/login", async (UserDTO request, IAuthServices services) =>
{
    var user = await services.LoginAsync(request);

    if (user == null)
    {
        return Results.BadRequest("Invalid username or password");
    }
    return Results.Ok(user);
});

app.MapPost("/todo", [Authorize] async Task<IResult>(HttpContext httpContext, ITodoServices todoServices, TodoDTO todos) =>
{
    var userIdClaim = httpContext.User.FindFirst(ClaimTypes.NameIdentifier);
    if (userIdClaim == null)
    {
        return Results.Unauthorized();
    }

    var todo2 = new Todo
    {
        Title = todos.Title,
        Description = todos.Description,
        IsCompleted = todos.IsCompleted,
        CreatedAt = DateTime.UtcNow,
        UpdatedAt = DateTime.UtcNow,
        UserId = Guid.Parse(userIdClaim.Value)
    };

    var todo = await todoServices!.AddTodoAsync(todo2);
    return TypedResults.CreatedAtRoute("GetTodo", todo);
});

app.MapGet("/todo", [Authorize] async (HttpContext httpContext, ITodoServices todoservice) =>
{
    var userIdClaim = httpContext.User.FindFirst(ClaimTypes.NameIdentifier);
    if (userIdClaim == null)
    {
        return Results.Unauthorized();
    }

    var todos = await todoservice.GetTodosbyIdAsync(Guid.Parse(userIdClaim.Value));
    return Results.Ok(todos);
});

app.MapPut("/todo/{id:Guid}", [Authorize] async Task<Results<Ok<Todo>, NotFound>>(Guid id, TodoDTO todoDto, HttpContext context, ITodoServices todoService) =>
{
    var userIdClaim = context.User.FindFirst(ClaimTypes.NameIdentifier);
    if (userIdClaim == null)
    {
        return TypedResults.NotFound();
    }
    var userId = Guid.Parse(userIdClaim.Value);

    // Fetch the existing todo to ensure it belongs to the user
    var existingTodo = await todoService.GetTodoAsync(id);
    if (existingTodo is null || existingTodo.UserId != userId)
    {
        return TypedResults.NotFound();
    }

    // Update fields
    existingTodo.Title = todoDto.Title;
    existingTodo.Description = todoDto.Description;
    existingTodo.IsCompleted = todoDto.IsCompleted;
    existingTodo.UpdatedAt = DateTime.UtcNow;

    var updatedTodo = await todoService.UpdateTodoAsync(id, existingTodo);
    if (updatedTodo is null)
    {
        return TypedResults.NotFound();
    }
    return TypedResults.Ok(updatedTodo);
});

app.MapGet("/todo/{id:Guid}", [Authorize] async Task<Results<Ok<Todo>, NotFound>>(Guid id, HttpContext httpContext, ITodoServices todoServices) =>
{
    var userIdClaim = httpContext.User.FindFirst(ClaimTypes.NameIdentifier);
    if (userIdClaim == null)
    {
        return TypedResults.NotFound();
    }
    try
    {
        var todo = await todoServices.GetTodoAsync(id);

        if (todo is null)
        {
            return TypedResults.NotFound();
        }
        if (todo.UserId != Guid.Parse(userIdClaim.Value))
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

app.MapDelete("/todo/{id:Guid}", [Authorize] async Task<Results<Ok, NotFound>>(Guid id, ITodoServices todoService, HttpContext context) =>
{
    var userIdClaim = context.User.FindFirst(ClaimTypes.NameIdentifier);
    if (userIdClaim == null)
    {
        return TypedResults.NotFound();
    }
    var userId = Guid.Parse(userIdClaim.Value);

    var todo = await todoService.DeleteTodoAsync(id, userId);
    if (todo is null)
    {
        return TypedResults.NotFound();
    }

    return TypedResults.Ok();
});

app.Run();


