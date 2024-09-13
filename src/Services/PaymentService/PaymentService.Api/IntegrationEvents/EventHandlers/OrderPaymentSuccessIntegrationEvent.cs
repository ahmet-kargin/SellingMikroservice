using EventBus.Base.Events;

namespace PaymentService.Api.IntegrationEvents.EventHandlers;

public class OrderPaymentSuccessIntegrationEvent : IntegrationEvent
{
    public Guid OrderId { get; }

    public OrderPaymentSuccessIntegrationEvent(Guid orderId) => OrderId = orderId;
}
