using EventBus.Base.Events;

namespace EventBus.Base.Abstraction
{
    public interface IEventBusSubscriptionManager
    {
        /// <summary>
        /// Check if empty
        /// </summary>
        bool IsEmpty { get; }
        /// <summary>
        /// Clears all subscriptions
        /// </summary>
        void Clear();
        /// <summary>
        /// Event removed handler
        /// </summary>
        event EventHandler<string> OnEventRemoved;
        /// <summary>
        /// Adds Subscription.
        /// </summary>
        /// <typeparam name="T">Type of IntegrationEvent</typeparam>
        /// <typeparam name="TH">Type of IIntegrationEventHandler</typeparam>
        void AddSubscription<T, TH>() where T : IntegrationEvent where TH : IIntegrationEventHandler<T>;
        /// <summary>
        /// Removes Subscription.
        /// </summary>
        /// <typeparam name="T">Type of IntegrationEvent</typeparam>
        /// <typeparam name="TH">Type of IIntegrationEventHandler</typeparam>
        void RemoveSubscription<T, TH>() where T : IntegrationEvent where TH : IIntegrationEventHandler<T>;
        /// <summary>
        /// Check if there are subscriptions for event by type of event.
        /// </summary>
        /// <typeparam name="T">Type of IntegrationEvent</typeparam>
        /// <returns>boolean</returns>
        bool HasSubscriptionsForEvent<T>() where T : IntegrationEvent;
        /// <summary>
        /// Check if there are subscriptions for event by event name in string form.
        /// </summary>
        /// <typeparam name="T">Type of IntegrationEvent</typeparam>
        /// <returns>boolean</returns>
        bool HasSubscriptionsForEvent(string eventName);
        /// <summary>
        /// Gets event type by event name in string form.
        /// </summary>
        /// <param name="eventName"></param>
        /// <returns></returns>
        Type GetEventTypeByName(string eventName);
        /// <summary>
        /// Returns handlers (subscriptions) for a event type
        /// </summary>
        /// <typeparam name="T">Type of IntegrationEvent</typeparam>
        /// <returns></returns>
        IEnumerable<SubscriptionInfo> GetHandlersForEvent<T>() where T : IntegrationEvent;
        /// <summary>
        /// Returns handlers (subscriptions) for a event name
        /// </summary>
        /// <typeparam name="T">Type of IntegrationEvent</typeparam>
        /// <returns>boolean</returns>
        IEnumerable<SubscriptionInfo> GetHandlersForEvent(string eventName);
        /// <summary>
        /// Returns name of the event.
        /// </summary>
        /// <typeparam name="T">Type of Event</typeparam>
        /// <returns>string</returns>
        string GetEventKey<T>();
    }
}
