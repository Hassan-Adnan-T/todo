namespace SimpleCrudApi.model.DTOs;

public class CreateTodoDto {
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
}