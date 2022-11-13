using EventBus.Base.Abstraction;
using EventBus.Base.Abstraction.SubscriptionManagers;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EventBus.Base.Events
{
    public abstract class BaseEventBus : IEventBus
    {
        public readonly IServiceProvider ServiceProvider;
        public readonly IEventBusSubscriptionManager SubscriptionManager;
        private EventBusConfig eventBusConfig;

        protected BaseEventBus(IServiceProvider serviceProvider, IEventBusSubscriptionManager subscriptionManager, EventBusConfig eventBusConfig)
        {
            this.eventBusConfig = eventBusConfig;
            ServiceProvider = serviceProvider;
            SubscriptionManager = new InMemoryEventBusSubscriptionManager(ProcessEventName);
        }

        private string ProcessEventName(string eventName)
        {
            if (eventBusConfig.DeleteEventPrefix)
                eventName = eventName.TrimStart(eventBusConfig.EventNamePrefix.ToArray());

            if (eventBusConfig.DeleteEventSuffix)
                eventName = eventName.TrimEnd(eventBusConfig.EventNameSuffix.ToArray());

            return eventName;
        }

        public virtual string GetSubName(string eventName)
        {
            return $"{eventBusConfig.SubscriberClientAppName}.{ProcessEventName(eventName)}";
        }

        public virtual void Dispose()
        {
            eventBusConfig = null;
        }

        public async Task<bool> ProcessEvent(string eventName, string message)
        {
            eventName = ProcessEventName(eventName);
            var processed = false;

            // Check if event listening
            if (SubscriptionManager.HasSubscriptionsForEvent(eventName))
            {
                // Get subscription info for event
                var subscriptions = SubscriptionManager.GetHandlersForEvent(eventName);

                // Create a dependency injection service provider scope for registering dynamically type of this services
                using (var scope = ServiceProvider.CreateScope())
                {
                    // loop all listeners
                    foreach (var sub in subscriptions)
                    {
                        // Get service for a this type of subscription handler type
                        var handler = ServiceProvider.GetService(sub.HandleType);
                        // if there is no service registered, it will continue to others
                        if (handler == null) continue;

                        // Get event type for originally created event name (before trimming)
                        var eventType = SubscriptionManager.GetEventTypeByName($"{eventBusConfig.EventNamePrefix}{eventName}{eventBusConfig.EventNameSuffix}");
                        // Deserialize message by type to event object
                        var integrationEvent = JsonConvert.DeserializeObject(message, eventType);

                        // Create generic type from event type
                        var concreteType = typeof(IIntegrationEventHandler<>).MakeGenericType(eventType);
                        // Lastly, execute to Handle method inside of this event
                        await (Task)concreteType.GetMethod("Handle").Invoke(handler, new object[] { integrationEvent });
                    }
                }

                processed = true;
            }

            return processed;
        }



        public abstract void Publish(IntegrationEvent @event);

        public abstract void Subscribe<T, TH>()
            where T : IntegrationEvent
            where TH : IIntegrationEventHandler<T>;

        public abstract void Unsubscribe<T, TH>()
            where T : IntegrationEvent
            where TH : IIntegrationEventHandler<T>;
    }
}
