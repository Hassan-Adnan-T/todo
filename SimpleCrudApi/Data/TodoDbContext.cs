using Microsoft.EntityFrameworkCore;
using SimpleCrudApi.model;

namespace SimpleCrudApi.Data;

public class TodoDbContext : DbContext {
    public TodoDbContext(DbContextOptions<TodoDbContext> options) : base(options) {}

    public DbSet<Todo> Todos { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder) {
        modelBuilder.Entity<Todo>(entity => {
             // Primary Key
            entity.HasKey(e => e.Id);
            
            // Required fields with max length
            entity.Property(e => e.Title)
                  .IsRequired()
                  .HasMaxLength(200);
            
            entity.Property(e => e.Description)
                  .HasMaxLength(1000);
            
            // Required timestamp
            entity.Property(e => e.CreatedAt)
                  .IsRequired();
        });
    }
}