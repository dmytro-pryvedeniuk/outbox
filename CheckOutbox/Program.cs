using CheckOutbox;
using JasperFx;
using JasperFx.Resources;
using Microsoft.EntityFrameworkCore;
using Wolverine;
using Wolverine.EntityFrameworkCore;
using Wolverine.Http;
using Wolverine.Postgresql;
using Wolverine.RabbitMQ;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<TasksContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("PostgreSQL")),
    ServiceLifetime.Singleton);

builder.Services.AddWolverine(options =>
{
    var config = builder.Configuration;
    var rabbitMqUri = builder.Configuration.GetConnectionString("RabbitMQ")!;

    options.UseRabbitMq(new Uri(rabbitMqUri))
        .AutoProvision()
        .UseConventionalRouting();

    options.Policies.DisableConventionalLocalRouting();

    options.PersistMessagesWithPostgresql(builder.Configuration.GetConnectionString("PostgreSQL")!);

    // This requires MapWolverineEnvelopeStorage() called in DBContext.OnModelCreating
    // https://wolverinefx.net/guide/durability/efcore/outbox-and-inbox.html#manually-adding-envelope-mapping
    options.UseEntityFrameworkCoreTransactions();

    options.Policies.AutoApplyTransactions();
    options.Policies.UseDurableInboxOnAllListeners();
    options.Policies.UseDurableOutboxOnAllSendingEndpoints();

    if (builder.Environment.IsDevelopment())
    {
        options.Durability.Mode = DurabilityMode.Solo;
    }
});

builder.Host.UseResourceSetupOnStartup();

builder.Services.AddWolverineHttp();

var app = builder.Build();

app.MapWolverineEndpoints();

app.Services.GetRequiredService<TasksContext>().Database.Migrate();

await app.RunJasperFxCommands(args);
