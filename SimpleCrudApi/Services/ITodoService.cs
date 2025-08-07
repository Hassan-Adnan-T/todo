using SimpleCrudApi.model;
using SimpleCrudApi.model.DTOs;

namespace SimpleCrudApi.Services;

public interface ITodoService 
{
    Task<IEnumerable<TodoResponseDto>> GetAllTodosAsync();
    Task<TodoResponseDto?> GetTodoByIdAsync(int id);
    Task<TodoResponseDto?> CreateTodoAsync(CreateTodoDto createTodoDto);
    Task<TodoResponseDto?> UpdateTodoAsync(int id, UpdateTodoDto updateTodoDto);
    Task<bool> DeleteTodoAsync(int id);
    Task<TodoResponseDto?> ToggleTodoCompletionAsync(int id);
}