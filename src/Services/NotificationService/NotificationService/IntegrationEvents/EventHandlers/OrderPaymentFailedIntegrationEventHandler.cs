using EventBus.Base.Abstraction;
using NotificationService.IntegrationEvents.Events;
using Serilog;

namespace NotificationService.IntegrationEvents.EventHandlers;

public class OrderPaymentFailedIntegrationEventHandler : IIntegrationEventHandler<OrderPaymentFailedIntegrationEvent>
{
    public Task Handle(OrderPaymentFailedIntegrationEvent @event)
    {
        // Send Fail Notification (Sms, EMail, Push)

        Log.Logger.Information($"Order Payment failed with OrderId: {@event.OrderId}, ErrorMessage: {@event.ErrorMessage}");

        return Task.CompletedTask;
    }
}
