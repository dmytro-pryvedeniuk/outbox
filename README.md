This repo demonstrates Outbox implementation with [Wolverine](https://github.com/JasperFx/wolverine), RabbitMQ, EntityFrameworkCore/PostgreSQL and Wolverine.Http.

- Run `docker-compose up -d` to start PostgreSQL and RabbitMQ.
- Start the app and send a request using `./CheckOutbox/CheckOutbox.http`.
  - `TaskItem` is stored in `Tasks` table.
  - `TaskItemCreated` message is stored in `wolverine.wolverine_outgoing_envelopes` table within the same transaction.
  - The request succeeds with `201 Created` status code.
  - `TaskItemCreated` message is published and removed from the table (almost instantly).

```
info: Microsoft.EntityFrameworkCore.Database.Command[20101]
      Executed DbCommand (9ms) [Parameters=[@p0='?' (DbType = Guid), @p1='?' (DbType = DateTime), @p2='?', @p3='?' (DbType = Guid), @p4='?' (DbType = Int32), @p5='?' (DbType = Binary), @p6='?' (DbType = DateTime), @p7='?', @p8='?', @p9='?' (DbType = Int32)], CommandType='Text', CommandTimeout='30']
      INSERT INTO "Tasks" ("Id", "CreatedAt", "Name")
      VALUES (@p0, @p1, @p2);
      INSERT INTO wolverine.wolverine_outgoing_envelopes (id, attempts, body, deliver_by, destination, message_type, owner_id)
      VALUES (@p3, @p4, @p5, @p6, @p7, @p8, @p9);
info: Wolverine.RabbitMQ.Internal.RabbitMqSender[0]
      Opened a new channel for Wolverine endpoint RabbitMqSender: rabbitmq://exchange/CheckOutbox.TaskItemCreated
info: CheckOutbox.TaskItemCreated[0]
      Task item created with Id: 019dee30-893b-7545-9f36-b0d123ebff43
info: CheckOutbox.TaskItemCreated[104]
      Successfully processed message CheckOutbox.TaskItemCreated#08dea91e-3fb8-950f-8c16-45bede0f0000 from rabbitmq://queue/CheckOutbox.TaskItemCreated
```

- Run `docker-compose stop rabbitmq` to stop RabbitMQ and send another request.
  - `TaskItem` is stored in `Tasks` table.
  - `TaskItemCreated` message is stored in `wolverine.wolverine_outgoing_envelopes` table within the same transaction.
  - The request succeeds with `201 Created` status code.
  - `TaskItemCreated` message can't be published and remains in `wolverine.wolverine_outgoing_envelopes` table.
  
```
info: Microsoft.EntityFrameworkCore.Database.Command[20101]
      Executed DbCommand (1ms) [Parameters=[@p0='?' (DbType = Guid), @p1='?' (DbType = DateTime), @p2='?', @p3='?' (DbType = Guid), @p4='?' (DbType = Int32), @p5='?' (DbType = Binary), @p6='?' (DbType = DateTime), @p7='?', @p8='?', @p9='?' (DbType = Int32)], CommandType='Text', CommandTimeout='30']
      INSERT INTO "Tasks" ("Id", "CreatedAt", "Name")
      VALUES (@p0, @p1, @p2);
      INSERT INTO wolverine.wolverine_outgoing_envelopes (id, attempts, body, deliver_by, destination, message_type, owner_id)
      VALUES (@p3, @p4, @p5, @p6, @p7, @p8, @p9);
fail: Wolverine.Persistence.Durability.DurableSendingAgent[201]
      Failed to send outgoing envelopes batch to rabbitmq://exchange/CheckOutbox.TaskItemCreated
```

- Run `docker-compose up -d` to start RabbitMQ again.
  - The connection is recovered automatically.
  - `TaskItemCreated` message is published and removed from the table.

```
info: Wolverine.RabbitMQ.Internal.RabbitMqTransport[0]
      RabbitMQ connection is recovered successfully
info: Wolverine.Persistence.Durability.DurableSendingAgent[204]
      Sending agent for rabbitmq://exchange/CheckOutbox.TaskItemCreated has resumed
....
info: CheckOutbox.TaskItemCreated[0]
      Task item created with Id: 019dee31-ab60-7894-b8be-018c9a99a35b
info: CheckOutbox.TaskItemCreated[104]
      Successfully processed message CheckOutbox.TaskItemCreated#08dea91e-6bec-043d-8c16-45bede0f0000 from rabbitmq://queue/CheckOutbox.TaskItemCreated
```

Both HTTP requests succeed. Both `TaskItem` instances are saved. Both `TaskItemCreated` messages reach the destination.
