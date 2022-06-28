namespace BasketballStats.Domain.Entities
{
    public class Stream
    {
        public long Id { get; init; }
        public Guid StreamId { get; init; }
        public Guid EventId { get; init; }
        public string Type { get; init; } = string.Empty;
        public string Data { get; init; } = string.Empty;
        public string MetaData { get; init; } = string.Empty;
        public DateTime CreatedAt { get; init; }
        public int Version { get; init; }
    }
}
