namespace EventBus.Base
{
    public class SubscriptionInfo
    {
        public Type HandleType { get; }

        public SubscriptionInfo(Type handleType)
        {
            HandleType = handleType ?? throw new ArgumentNullException(nameof(handleType));
        }

        // Dışarıdan statik olarak SubscriptionInfo çağırarak HandlerType oluşturabilmek için.
        public static SubscriptionInfo Typed(Type handlerType)
        {
            return new SubscriptionInfo(handlerType);
        }
    }
}
