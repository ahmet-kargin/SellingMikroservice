using BasketService.Api.Core.Application.Repository;
using BasketService.Api.Extensions;
using BasketService.Api.Infrastructure.Repository;
using Consul;
using EventBus.Base.Abstraction;
using EventBus.Base;
using IdentityService.Api.Application.Services;
using Microsoft.AspNetCore.Connections;
using static EventBus.Base.EventBusConfig;
using RabbitMQ.Client;
using EventBus.Factory;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddHttpContextAccessor();

builder.Services.AddTransient<IBasketRepository, RedisBasketRepository>();
builder.Services.AddTransient<IIdentityService, IdentityService.Api.Application.Services.IdentityService>();
builder.Services.AddTransient<BasketService.Api.IntegrationEvents.Events.OrderCreatedIntegrationEvent>();
builder.Services.AddSingleton<IEventBus>(sp =>
{
    EventBusConfig config = new()
    {
        ConnectionRetryCount = 5,
        EventNameSuffix = "IntegrationEvent",
        SubscriberClientAppName = "BasketService",
        EventBusTypes = EventBusType.RabbitMQ, // Düzeltildi: EventBusType

        Connection = new ConnectionFactory()
        {
            HostName = "localhost"
            // Diðer ayarlar: 
            // Port = 15672,
            // UserName = "guest",
            // Password = "guest",
            // VirtualHost="/"
        }
    };

    return EventBusFactory.Create(config, sp);
});

void ConfigureSubscription(IServiceProvider serviceProvider)
{
    var eventBus = serviceProvider.GetRequiredService<IEventBus>();
    eventBus.Subscribe<BasketService.Api.IntegrationEvents.Events.OrderCreatedIntegrationEvent, BasketService.Api.IntegrationEvents.EventHandlers.OrderCreatedIntegrationEventHandler>();
}
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.RegisterWithConsul(app.Lifetime, builder.Configuration);
app.MapControllers();

app.Run();


