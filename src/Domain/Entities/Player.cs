namespace BasketballStats.Domain.Entities;

public class Player
{
    public long Id { get; init; }
    public Guid PlayerId { get; init; } = Guid.NewGuid();
    public string Name { get; init; } = string.Empty;
    public string? MiddleName { get; init; }
    public string Surname { get; init; } = string.Empty;
    public DateOnly BirthDate { get; init; }
    public string Nationality { get; init; } = null!;
    public short Weight { get; init; } = short.MinValue;
    public short Height { get; init; } = short.MinValue;

    public virtual Team Team { get; init; } = null!;
}
