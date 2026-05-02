using Wolverine.Http;

namespace CheckOutbox;

public record CreateTask(string Name);

public class CreateTaskEndpoint
{
    [WolverinePost("tasks")]
    public (CreationResponse, TaskItemCreated) HandleAsync(CreateTask command, TasksContext context)
    {
        var taskItem = new TaskItem(Guid.CreateVersion7(), command.Name, DateTimeOffset.UtcNow);

        context.Tasks.Add(taskItem);

        return (new CreationResponse($"api/tasks/{taskItem.Id}"), new TaskItemCreated(taskItem.Id));
    }
}
