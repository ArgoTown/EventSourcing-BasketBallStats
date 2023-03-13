using BasketballStats.Domain;
using BasketballStats.Domain.Aggregate;
using BasketballStats.Domain.Entities;
using BasketballStats.Domain.Events;
using BasketballStats.Domain.Repositories;
using BasketballStats.Domain.Services;
using BasketballStats.Infrastructure.Repositories;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Text.Json;
using Player = BasketballStats.Domain.Aggregate.Player;

namespace BasketballStats.Infrastructure.Synchronizer;

internal class ReadModelBackgroundService : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ITypeResolverService _typeResolver;
    private int _totalReadEventsCount;

    public ReadModelBackgroundService(IServiceProvider serviceProvider, ITypeResolverService typeResolver)
    {
        _serviceProvider = serviceProvider;
        _typeResolver = typeResolver;
    }

    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        Task.Run(() => ReadModelProjectionFromStreams(stoppingToken), stoppingToken);
        return Task.CompletedTask;
    }

    private async Task ReadModelProjectionFromStreams(CancellationToken cancellationToken)
    {
        using var scope = _serviceProvider.CreateScope();
        var eventStoreRepository = scope.ServiceProvider.GetRequiredService<IEventStoreRepository>();

        while (!cancellationToken.IsCancellationRequested)
        {
            var events = await eventStoreRepository.GetAll(_totalReadEventsCount);
            if (events.Any())
            {
                foreach (var @event in events)
                {
                    var metadata = JsonSerializer.Deserialize<Metadata>(@event.MetaData)!;
                    var player = new Player(@event.StreamId, metadata.TeamId, metadata.PlayerId);
                    var aggregate = new PlayerAggregate(player);
                    aggregate.ApplyEvent((IEvent)JsonSerializer.Deserialize(
                        @event.Data,
                        _typeResolver.GetTypeByEventName(@event.Type),
                        Constants.EnumSerializerOptions)!);

                    await UpdateReadModel(scope, aggregate);
                    _totalReadEventsCount++;
                }
            }
        }
    }

    private async Task UpdateReadModel(IServiceScope scope, PlayerAggregate aggregate)
    {
        var gameStatsReadModelRepository = scope.ServiceProvider.GetRequiredService<IGameStatsReadModel>();

        var streamEntities = await gameStatsReadModelRepository.Get(aggregate.State.GameId);

        var playerEntity = streamEntities.FirstOrDefault(streamEntity => streamEntity.PlayerId.Equals(aggregate.State.Id) && streamEntity.TeamId.Equals(aggregate.State.TeamId));
        if (playerEntity is not null)
        {
            playerEntity.Blocks += aggregate.State.Blocks;
            playerEntity.BlocksReceived += aggregate.State.BlocksReceived;
            playerEntity.DefensiveRebounds += aggregate.State.DefensiveRebounds;
            playerEntity.Fouls += aggregate.State.Fouls;
            playerEntity.FoulsProvoked += aggregate.State.FoulsProvoked;
            playerEntity.MadeFreeThrows += aggregate.State.MadeFreeThrows;
            playerEntity.MadeThreePoints += aggregate.State.MadeThreePoints;
            playerEntity.MadeTwoPoints += aggregate.State.MadeTwoPoints;
            playerEntity.MissedFreeThrows += aggregate.State.MissedFreeThrows;
            playerEntity.MissedThreePoints += aggregate.State.MissedThreePoints;
            playerEntity.MissedTwoPoints += aggregate.State.MissedTwoPoints;
            playerEntity.OffensiveRebounds += aggregate.State.OffensiveRebounds;
            playerEntity.Steals += aggregate.State.Steals;
            playerEntity.Turnovers += aggregate.State.Turnovers;

            await gameStatsReadModelRepository.Update(playerEntity);
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

            await gameStatsReadModelRepository.Add(entity);
        }
    }
}
