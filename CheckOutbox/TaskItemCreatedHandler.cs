namespace CheckOutbox;

public static class TaskItemCreatedHandler
{
    public static void Handle(TaskItemCreated itemCreated, ILogger<TaskItemCreated> logger)
    {
        logger.LogInformation("Task item created with Id: {Id}", itemCreated.Id);
    }
}
