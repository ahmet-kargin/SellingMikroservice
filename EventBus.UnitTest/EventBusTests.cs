using EventBus.Base;
using EventBus.Base.Abstraction;
using EventBus.Factory;
using EventBus.UnitTest.Events.EventHandlers;
using EventBus.UnitTest.Events.Events;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RabbitMQ.Client;
using System.Threading.Tasks;

namespace EventBus.UnitTest
{
    public class EventBusTests
    {
        private readonly ServiceCollection services;

        public EventBusTests()
        {
            services = new ServiceCollection();
            services.AddLogging(configure => configure.AddConsole());
        }

        [Fact]
        public void Subscribe_Event_On_RabbitMQ_Test()
        {
            services.AddSingleton<IEventBus>(sp =>
            {
                return EventBusFactory.Create(GetRabbitMQConfig(), sp);
            });

            var sp = services.BuildServiceProvider();
            var eventBus = sp.GetRequiredService<IEventBus>();

            eventBus.Subscribe<OrderCreatedIntegrationEvent, OrderCreatedIntegrationEventHandler>();
            // eventBus.UnSubscribe<OrderCreatedIntegrationEvent, OrderCreatedIntegrationEventHandler>();
        }

        [Fact]
        public void Subscribe_Event_On_Azure_Test()
        {
            services.AddSingleton<IEventBus>(sp =>
            {
                return EventBusFactory.Create(GetAzureConfig(), sp);
            });

            var sp = services.BuildServiceProvider();
            var eventBus = sp.GetRequiredService<IEventBus>();

            eventBus.Subscribe<OrderCreatedIntegrationEvent, OrderCreatedIntegrationEventHandler>();
            // eventBus.UnSubscribe<OrderCreatedIntegrationEvent, OrderCreatedIntegrationEventHandler>();

            Task.Delay(2000).Wait(); // Bu, async kodu simüle etmek için kullanýlýyor.
        }

        [Fact]
        public void Send_Message_To_RabbitMQ_Test()
        {
            services.AddSingleton<IEventBus>(sp =>
            {
                return EventBusFactory.Create(GetRabbitMQConfig(), sp);
            });

            var sp = services.BuildServiceProvider();
            var eventBus = sp.GetRequiredService<IEventBus>();

            eventBus.Publish(new OrderCreatedIntegrationEvent(1));
        }

        [Fact]
        public void Send_Message_To_Azure_Test()
        {
            services.AddSingleton<IEventBus>(sp =>
            {
                return EventBusFactory.Create(GetAzureConfig(), sp);
            });

            var sp = services.BuildServiceProvider();
            var eventBus = sp.GetRequiredService<IEventBus>();

            eventBus.Publish(new OrderCreatedIntegrationEvent(1));
        }

        private EventBusConfig GetAzureConfig()
        {
            return new EventBusConfig()
            {
                ConnectionRetryCount = 5,
                SubscriberClientAppName = "EventBus.UnitTest",
                DefaultTopicName = "SellingBuddyTopicName",
                EventBusTypes = EventBusConfig.EventBusType.AzureServiceBus,
                EventNameSuffix = "IntegrationEvent",
                EventBusConnectionString = "Endpoint=sb://techbuddy.servicebus.windows.net/;SharedAccessKeyName=NewPolicyForYTVideos;SharedAccessKey=7sJghGWFOXaUaRblrbzOIIf4bQk6qkbTN/SEnKjXLpE="
            };
        }

        private EventBusConfig GetRabbitMQConfig()
        {
            return new EventBusConfig()
            {
                ConnectionRetryCount = 5,
                SubscriberClientAppName = "EventBus.UnitTest",
                DefaultTopicName = "SellingBuddyTopicName",
                EventBusTypes = EventBusConfig.EventBusType.RabbitMQ,
                EventNameSuffix = "IntegrationEvent",
                // Connection = new ConnectionFactory()
                // { 
                //    HostName = "localhost",
                //    Port = 15672,
                //    UserName = "guest",
                //    Password = "guest"
                // }
            };
        }
    }
}
