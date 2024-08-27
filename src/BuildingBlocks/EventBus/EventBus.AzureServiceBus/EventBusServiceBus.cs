using EventBus.Base;
using EventBus.Base.Events;
using Microsoft.Azure.ServiceBus;
using Microsoft.Azure.ServiceBus.Management;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EventBus.AzureServiceBus;

public class EventBusServiceBus : BaseEventBus
{
    private ITopicClient topicClient;
    private ManagementClient managementClient;
    private ILogger logger;
    public EventBusServiceBus(EventBusConfig config, IServiceProvider serviceProvider) : base(config, serviceProvider)
    {
        managementClient = new ManagementClient(config.EventBusConnectionString);
        topicClient = createTopicClient();
        logger = serviceProvider.GetService(typeof(ILogger<EventBusServiceBus>)) as ILogger<EventBusServiceBus>;
    }
    public override void Publish(IntegrationEvent @event)
    {
        var eventName = @event.GetType().Name;

        eventName = ProcessEventName(eventName);

        var eventStr = JsonConvert.SerializeObject(@event);
        var bodyArr = Encoding.UTF8.GetBytes(eventStr);

        var message = new Message()
        {
            MessageId = Guid.NewGuid().ToString(),
            Body = bodyArr,
            Label = eventName
        };
        topicClient.SendAsync(message).GetAwaiter().GetResult();
    }

    private ITopicClient createTopicClient()
    {
        if (topicClient == null || topicClient.IsClosedOrClosing)
        {
            topicClient = new TopicClient(EventBusConfig.EventBusConnectionString, EventBusConfig.DefaultTopicName, RetryPolicy.Default);
        }
        if (!managementClient.TopicExistsAsync(EventBusConfig.DefaultTopicName).GetAwaiter().GetResult())
            managementClient.CreateTopicAsync(EventBusConfig.DefaultTopicName).GetAwaiter().GetResult();

        return topicClient;
    }

    public override void Subscribe<T, TH>()
    {
        var eventName = typeof(T).Name;
        eventName = ProcessEventName(eventName);

        if (!SubsManager.HasSubscriptionsForEvent(eventName))
        {
            var subscriptionClient = CreateSubscriptionClientIfNotExists(eventName);

            RegisterSubscriptionClientMessageHandler(subscriptionClient);
        }

        logger.LogInformation("Subscribing to event {EventName} with {EventHandler}", eventName,typeof(TH).Name);
        SubsManager.AddSubscription<T, TH>();
    }

    private ISubscriptionClient CreateSubscriptionClientIfNotExists(string eventName)
    {
        var subClient = CreateSubscriptionClient(eventName);

        var exists = managementClient.CreateSubscriptionAsync(EventBusConfig.DefaultTopicName, GetSubName(eventName)).GetAwaiter().GetResult();
        if (exists == null)
        {
            managementClient.CreateSubscriptionAsync(EventBusConfig.DefaultTopicName, GetSubName(eventName)).GetAwaiter().GetResult();
            RemoveDefaultRule(subClient);
        }
        CreateRuleIfNotExists(ProcessEventName(eventName), subClient);

        return subClient;
    }

    private void RegisterSubscriptionClientMessageHandler(ISubscriptionClient subscriptionClient)
    {
        subscriptionClient.RegisterMessageHandler(
            async (message, token) =>
            {
                var eventName = $"{message.Label}";
                var messageData = Encoding.UTF8.GetString(message.Body);

                if (await ProcessEvent(ProcessEventName(eventName), messageData))
                {
                    await subscriptionClient.CompleteAsync(message.SystemProperties.LockToken);
                }

            },
            new MessageHandlerOptions(ExceptionRecievedHandler) { MaxConcurrentCalls = 10, AutoComplete = false });
    }

    private Task ExceptionRecievedHandler(ExceptionReceivedEventArgs exceptionReceivedEventArgs)
    { 
        var ex = exceptionReceivedEventArgs.Exception;
        var context = exceptionReceivedEventArgs.ExceptionReceivedContext;

        logger.LogError(ex,"ERROR handling message: {ExcepitonMessage} - Context: {@ExceptionContext}",ex.Message,context);
        return Task.CompletedTask;
    }

    private void CreateRuleIfNotExists(string eventName, ISubscriptionClient subscriptionClient)
    {
        bool ruleExists;
        try
        {
            var rule = managementClient.GetRuleAsync(EventBusConfig.DefaultTopicName, eventName, eventName).GetAwaiter().GetResult();
            ruleExists = rule != null;
        }
        catch (MessagingEntityNotFoundException)
        {

            ruleExists = false;
        }

        if (!ruleExists)
        {
            subscriptionClient.AddRuleAsync(new RuleDescription
            {
                Filter = new CorrelationFilter { Label = eventName },
                Name = eventName,
            }).GetAwaiter().GetResult();
        }
    }

    private void RemoveDefaultRule(SubscriptionClient subscriptionClient)
    {
        try
        {
            subscriptionClient
                    .RemoveRuleAsync(RuleDescription.DefaultRuleName)
                    .GetAwaiter().GetResult();
        }
        catch (Exception)
        {

            logger.LogWarning("The messaging entity {DefaultRuleName} Could not be found.", RuleDescription.DefaultRuleName);
        }
    }
    private SubscriptionClient CreateSubscriptionClient(string eventName)
    {
        return new SubscriptionClient(EventBusConfig.EventBusConnectionString, EventBusConfig.DefaultTopicName, GetSubName(eventName));
    }

    public override void UnSubscribe<T, TH>()
    {
        var eventName = typeof(T).Name;

        try
        {
            var subscriptionClient = CreateSubscriptionClient(eventName);

            subscriptionClient
                    .RemoveRuleAsync(RuleDescription.DefaultRuleName)
                    .GetAwaiter().GetResult();
        }
        catch (Exception)
        {

            logger.LogWarning("The messaging entity {EventName} Could not be found.", eventName);
        }

        logger.LogInformation("Unsubscribing from evetn {EventName}",eventName);

        SubsManager.RemoveSubscription<T, TH>();   
    }

    public override void Dispose()
    {
        base.Dispose();
        topicClient.CloseAsync().GetAwaiter().GetResult();
        managementClient.CloseAsync().GetAwaiter().GetResult();
        topicClient = null;
        managementClient = null;
    }
}
