using Microsoft.EntityFrameworkCore;
using SimpleCrudApi.Data;
using SimpleCrudApi.model.DTOs;
using SimpleCrudApi.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddOpenApi();

// Add Swagger services
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new() { Title = "Todo API", Version = "v1" });
});

// Add Database
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") 
    ?? "Data Source=todos.db";
builder.Services.AddDbContext<TodoDbContext>(options =>
    options.UseSqlite(connectionString));

// Add services
builder.Services.AddScoped<ITodoService, TodoService>();

// Add CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Todo API v1");
        c.RoutePrefix = string.Empty; // Serve Swagger UI at root
    });
}

app.UseCors("AllowAll");
app.UseHttpsRedirection();

// Ensure database is created
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<TodoDbContext>();
    context.Database.EnsureCreated();
}

// API Endpoints
var todos = app.MapGroup("/api/todos").WithTags("Todos");

// GET /api/todos - Get all todos
todos.MapGet("/", async (ITodoService todoService) =>
{
    var allTodos = await todoService.GetAllTodosAsync();
    return Results.Ok(allTodos);
})
.WithName("GetAllTodos")
.WithSummary("Get all todos")
.Produces<IEnumerable<TodoResponseDto>>();

// GET /api/todos/{id} - Get todo by ID
todos.MapGet("/{id:int}", async (int id, ITodoService todoService) =>
{
    var todo = await todoService.GetTodoByIdAsync(id);
    return todo == null ? Results.NotFound() : Results.Ok(todo);
})
.WithName("GetTodoById")
.WithSummary("Get a todo by ID")
.Produces<TodoResponseDto>()
.Produces(404);

// POST /api/todos - Create new todo
todos.MapPost("/", async (CreateTodoDto createDto, ITodoService todoService) =>
{
    if (string.IsNullOrWhiteSpace(createDto.Title))
        return Results.BadRequest("Title is required");

    var todo = await todoService.CreateTodoAsync(createDto);
    return Results.Created($"/api/todos/{todo?.Id}", todo);
})
.WithName("CreateTodo")
.WithSummary("Create a new todo")
.Produces<TodoResponseDto>(201)
.Produces(400);

// PUT /api/todos/{id} - Update todo
todos.MapPut("/{id:int}", async (int id, UpdateTodoDto updateDto, ITodoService todoService) =>
{
    var todo = await todoService.UpdateTodoAsync(id, updateDto);
    return todo == null ? Results.NotFound() : Results.Ok(todo);
})
.WithName("UpdateTodo")
.WithSummary("Update a todo")
.Produces<TodoResponseDto>()
.Produces(404);

// DELETE /api/todos/{id} - Delete todo
todos.MapDelete("/{id:int}", async (int id, ITodoService todoService) =>
{
    var deleted = await todoService.DeleteTodoAsync(id);
    return deleted ? Results.NoContent() : Results.NotFound();
})
.WithName("DeleteTodo")
.WithSummary("Delete a todo")
.Produces(204)
.Produces(404);

// PATCH /api/todos/{id}/toggle - Toggle todo completion
todos.MapPatch("/{id:int}/toggle", async (int id, ITodoService todoService) =>
{
    var todo = await todoService.ToggleTodoCompletionAsync(id);
    return todo == null ? Results.NotFound() : Results.Ok(todo);
})
.WithName("ToggleTodo")
.WithSummary("Toggle todo completion status")
.Produces<TodoResponseDto>()
.Produces(404);

// Health check endpoint
app.MapGet("/health", () => Results.Ok(new { Status = "Healthy", Timestamp = DateTime.UtcNow }))
.WithName("HealthCheck")
.WithSummary("Health check endpoint");

app.Run();


