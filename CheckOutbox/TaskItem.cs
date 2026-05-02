namespace CheckOutbox;

public record TaskItem(Guid Id, string Name, DateTimeOffset CreatedAt);
