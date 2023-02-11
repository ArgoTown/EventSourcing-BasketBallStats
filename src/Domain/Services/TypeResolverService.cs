using BasketballStats.Domain.Events;

namespace BasketballStats.Domain.Services;

internal sealed class TypeResolverService : ITypeResolverService
{
    private static readonly Dictionary<string, Type> MapFromStringToType = new()
    {
        { "NegativeEventHappened", typeof(NegativeEventHappened) },
        { "PositiveEventHappened", typeof(PositiveEventHappened) }
    };

    private static readonly Dictionary<Type, string> MapFromTypeToString = new()
    {
        { typeof(NegativeEventHappened), "NegativeEventHappened" },
        { typeof(PositiveEventHappened), "PositiveEventHappened" }
    };

    public Type GetTypeByEventName(string eventName) => MapFromStringToType[eventName];
    public string GetEventNameByType(Type eventType) => MapFromTypeToString[eventType];
}
