using Microsoft.EntityFrameworkCore;
using Wolverine.EntityFrameworkCore;

namespace CheckOutbox;

public class TasksContext(DbContextOptions<TasksContext> options) : DbContext(options)
{
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.MapWolverineEnvelopeStorage("wolverine");
        base.OnModelCreating(modelBuilder);
    }

    public DbSet<TaskItem> Tasks => Set<TaskItem>();
}
