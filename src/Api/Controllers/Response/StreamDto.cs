namespace BasketballStats.Api.Controllers.Response
{
    /// <summary>
    /// Stream details about event.
    /// </summary>
    public record StreamDto(long Id, Guid StreamId, Guid EventId, string Type, string Data, string MetaData, DateTime CreatedAt, int Version);
}
