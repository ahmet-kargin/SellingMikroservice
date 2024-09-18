using OrderService.Api.IntegrationEvents.EventHandlers;

namespace OrderService.Api.Extentions.Registration.EventHandlerRegistration;

public static class EventHandlerRegistration
{
    public static IServiceCollection ConfigureEventHandlers(this IServiceCollection services)
    {
        services.AddTransient<OrderCreatedIntegrationEventHandler>();

        return services;
    }
}
