using Microsoft.EntityFrameworkCore;
using SimpleCrudApi.Data;
using SimpleCrudApi.model;
using SimpleCrudApi.model.DTOs;
using SimpleCrudApi.Services;

namespace SimpleCrudApi.Services;

public class TodoService : ITodoService {
    private readonly TodoDbContext _context;

    public TodoService(TodoDbContext context) {
        _context = context;
    }

    // Get all todos
    public async Task<IEnumerable<TodoResponseDto>> GetAllTodosAsync() {
        var todos = await _context.Todos.ToListAsync();
        return todos.Select(MaptoResponseDto);
    }

    // Get todo by id
    public async Task<TodoResponseDto?> GetTodoByIdAsync(int id) {
        var todo = await _context.Todos.FindAsync(id);
        return todo == null ? null : MaptoResponseDto(todo);
    }

    // Create todo
    public async Task<TodoResponseDto?> CreateTodoAsync(CreateTodoDto createTodoDto) {
        var todo = new Todo 
        {
            Title = createTodoDto.Title,
            Description = createTodoDto.Description,
            IsCompleted = false,
            CreatedAt = DateTime.UtcNow
        };

        _context.Todos.Add(todo);
        await _context.SaveChangesAsync();
        return MaptoResponseDto(todo); 
    }

    // Update todo
    public async Task<TodoResponseDto?> UpdateTodoAsync(int id, UpdateTodoDto updateTodoDto) {
        var todo = await _context.Todos.FindAsync(id);
        if (todo == null) return null;

        if (!string.IsNullOrEmpty(updateTodoDto.Title))
            todo.Title = updateTodoDto.Title;
        
        if (updateTodoDto.Description != null)
            todo.Description = updateTodoDto.Description;

        if (updateTodoDto.IsCompleted.HasValue) {

           todo.IsCompleted = updateTodoDto.IsCompleted.Value;
            if (todo.IsCompleted && todo.CompletedAt == null)
                todo.CompletedAt = DateTime.UtcNow;
            else if (!todo.IsCompleted)
                todo.CompletedAt = null;
        }

        await _context.SaveChangesAsync();
        return MaptoResponseDto(todo);
    }

    // Delete todo
    public async Task<bool> DeleteTodoAsync(int id) {
        var todo = await _context.Todos.FindAsync(id);
        if (todo == null) return false;

        _context.Todos.Remove(todo);
        await _context.SaveChangesAsync();
        return true;
    }

    // Toggle todo completion
    public async Task<TodoResponseDto?> ToggleTodoCompletionAsync(int id) {
        var todo = await _context.Todos.FindAsync(id);
        if (todo == null) return null;

        todo.IsCompleted = !todo.IsCompleted;
        todo.CompletedAt = todo.IsCompleted ? DateTime.UtcNow : null;

        await _context.SaveChangesAsync();
        return MaptoResponseDto(todo);
    }

    private static TodoResponseDto MaptoResponseDto(Todo todo) {
        return new TodoResponseDto {
            Id = todo.Id,
            Title = todo.Title,
            Description = todo.Description,
            IsCompleted = todo.IsCompleted,
            CreatedAt = todo.CreatedAt,
            CompletedAt = todo.CompletedAt
        }
    }
}