namespace BasketballStats.Domain.Services
{
    public interface ITypeResolverService
    {
        Type GetTypeByEventName(string eventName);
        string GetEventNameByType(Type eventType);
    }
}
