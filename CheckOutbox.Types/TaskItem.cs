namespace CheckOutbox.Types;

public record TaskItem(Guid Id, string Name, DateTimeOffset CreatedAt);
