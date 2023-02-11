using BasketballStats.Domain.Aggregate;

namespace BasketballStats.Domain.Repositories;

public interface IEventStoreRepository
{
    Task Add(PlayerAggregate playerAggregate);
    Task<IReadOnlyList<Entities.Stream>> Get(Guid gameId);
    Task<IReadOnlyList<Entities.Stream>> GetAll(int skipEvents = default);
}
