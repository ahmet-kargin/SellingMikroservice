using EventBus.Base.Abstraction;
using EventBus.Base;
using EventBus.Factory;
using PaymentService.Api.IntegrationEvents.EventHandlers;
using RabbitMQ.Client;
using static EventBus.Base.EventBusConfig;
using PaymentService.Api.IntegrationEvents.Events;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddTransient<OrderStartedIntegrationEventHandler>();

builder.Services.AddSingleton<IEventBus>(sp =>
{
    EventBusConfig config = new()
    {
        ConnectionRetryCount = 5,
        EventNameSuffix = "IntegrationEvent",
        SubscriberClientAppName = "PaymentService",
        EventBusTypes = EventBusType.RabbitMQ,
        Connection = new ConnectionFactory()
        {
            HostName = "localhost",
            Port = 15672,
            UserName = "guest",
            Password = "guest"
        },
        //Connection = new ConnectionFactory()
        //{
        //    HostName = "c_rabbitmq"
        //}
    };

    return EventBusFactory.Create(config, sp);
});
builder.Services.AddLogging(configure =>
{
    configure.AddConsole();
    configure.AddDebug();
});
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}


using (var scope = app.Services.CreateScope())
{
    var eventBus = scope.ServiceProvider.GetRequiredService<IEventBus>();
    eventBus.Subscribe<OrderStartedIntegrationEvent, OrderStartedIntegrationEventHandler>();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
