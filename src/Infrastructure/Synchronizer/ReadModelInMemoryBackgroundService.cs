using BasketballStats.Domain;
using BasketballStats.Domain.Aggregate;
using BasketballStats.Domain.Entities;
using BasketballStats.Domain.Events;
using BasketballStats.Domain.Repositories;
using BasketballStats.Domain.Services;
using BasketballStats.Infrastructure.Repositories;
using Microsoft.Extensions.Hosting;
using System.Text.Json;

namespace BasketballStats.Infrastructure.Synchronizer
{
    public class ReadModelInMemoryBackgroundService : BackgroundService
    {
        private readonly IEventStoreRepository _eventStoreRepository;
        private readonly IGameStatsReadModel _statisticsReadModel;
        private readonly ITypeResolverService _typeResolver;
        private int _totalReadEventsCount;

        public ReadModelInMemoryBackgroundService(
            IEventStoreRepository eventStoreRepository,
            IGameStatsReadModel statisticsReadModel,
            ITypeResolverService typeResolver)
        {
            _eventStoreRepository = eventStoreRepository;
            _statisticsReadModel = statisticsReadModel;
            _typeResolver = typeResolver;
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            return Task.Run(() => ReadModelProjectionFromStreams(stoppingToken), stoppingToken);
        }

        private async Task ReadModelProjectionFromStreams(CancellationToken cancellationToken)
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                var events = await _eventStoreRepository.GetAll(_totalReadEventsCount);
                if (events.Any())
                {
                    foreach (var @event in events)
                    {
                        var metadata = JsonSerializer.Deserialize<Metadata>(@event.MetaData)!;
                        var player = new Domain.Aggregate.Player(@event.StreamId, metadata.TeamId, metadata.PlayerId);
                        var aggregate = new PlayerAggregate(player);
                        aggregate.ApplyEvent((IEvent)JsonSerializer.Deserialize(
                            @event.Data,
                            _typeResolver.GetTypeByEventName(@event.Type),
                            Constants.EnumSerializerOptions)!);

                        await UpdateReadModel(aggregate);
                        _totalReadEventsCount++;
                    }
                }
            }
        }

        private async Task UpdateReadModel(PlayerAggregate aggregate)
        {
            var streamEntities = await _statisticsReadModel.Get(aggregate.State.GameId);

            var playerEntity = streamEntities.FirstOrDefault(streamEntity => streamEntity.PlayerId.Equals(aggregate.State.Id) && streamEntity.TeamId.Equals(aggregate.State.TeamId));
            if (playerEntity is not null)
            {
                var entity = new GameStatsReadModel()
                {
                    Id = playerEntity.Id,
                    Blocks = (short)(aggregate.State.Blocks + playerEntity.Blocks),
                    BlocksReceived = (short)(aggregate.State.BlocksReceived + playerEntity.BlocksReceived),
                    DefensiveRebounds = (short)(aggregate.State.DefensiveRebounds + playerEntity.DefensiveRebounds),
                    Fouls = (short)(aggregate.State.Fouls + playerEntity.Fouls),
                    FoulsProvoked = (short)(aggregate.State.FoulsProvoked + playerEntity.FoulsProvoked),
                    MadeFreeThrows = (short)(aggregate.State.MadeFreeThrows + playerEntity.MadeFreeThrows),
                    MadeThreePoints = (short)(aggregate.State.MadeThreePoints + playerEntity.MadeThreePoints),
                    MadeTwoPoints = (short)(aggregate.State.MadeTwoPoints + playerEntity.MadeTwoPoints),
                    MissedFreeThrows = (short)(aggregate.State.MissedFreeThrows + playerEntity.MissedFreeThrows),
                    MissedThreePoints = (short)(aggregate.State.MissedThreePoints + playerEntity.MissedThreePoints),
                    MissedTwoPoints = (short)(aggregate.State.MissedTwoPoints + playerEntity.MissedTwoPoints),
                    OffensiveRebounds = (short)(aggregate.State.OffensiveRebounds + playerEntity.OffensiveRebounds),
                    Steals = (short)(aggregate.State.Steals + playerEntity.Steals),
                    Turnovers = (short)(aggregate.State.Turnovers + playerEntity.Turnovers),
                    GameId = aggregate.State.GameId,
                    PlayerId = aggregate.State.Id,
                    TeamId = aggregate.State.TeamId
                };

                await _statisticsReadModel.Update(entity);
            }
            else
            {
                var entity = new GameStatsReadModel()
                {
                    Blocks = aggregate.State.Blocks,
                    BlocksReceived = aggregate.State.BlocksReceived,
                    DefensiveRebounds = aggregate.State.DefensiveRebounds,
                    Fouls = aggregate.State.Fouls,
                    FoulsProvoked = aggregate.State.FoulsProvoked,
                    GameId = aggregate.State.GameId,
                    MadeFreeThrows = aggregate.State.MadeFreeThrows,
                    MadeThreePoints = aggregate.State.MadeThreePoints,
                    MadeTwoPoints = aggregate.State.MadeTwoPoints,
                    MissedFreeThrows = aggregate.State.MissedFreeThrows,
                    MissedThreePoints = aggregate.State.MissedThreePoints,
                    MissedTwoPoints = aggregate.State.MissedTwoPoints,
                    OffensiveRebounds = aggregate.State.OffensiveRebounds,
                    Steals = aggregate.State.Steals,
                    Turnovers = aggregate.State.Turnovers,
                    PlayerId = aggregate.State.Id,
                    TeamId = aggregate.State.TeamId
                };

                await _statisticsReadModel.Add(entity);
            }
        }
    }
}
