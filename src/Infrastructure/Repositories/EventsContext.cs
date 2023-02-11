using BasketballStats.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace BasketballStats.Infrastructure.Repositories;

public class EventsContext : DbContext
{
    public DbSet<Domain.Entities.Stream> Streams { get; set; }
    public DbSet<Game> Games { get; set; }
    public DbSet<Team> Teams { get; set; }
    public DbSet<Player> Players { get; set; }
    public DbSet<Arena> Arenas { get; set; }
    public DbSet<GameStatsReadModel> ReadModelStatistics { get; set; }

    public EventsContext(DbContextOptions<EventsContext> options)
    : base(options) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        StreamDataModelBuilder(modelBuilder);
        GamDataModelBuilder(modelBuilder);
        ArenaDataModelBuilder(modelBuilder);
        TeamDataModelBuilder(modelBuilder);
        DataModelsIndexesBuilder(modelBuilder);
        ReadModelStatisticsDataModelBuilder(modelBuilder);
    }

    private static void ReadModelStatisticsDataModelBuilder(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<GameStatsReadModel>()
                        .Property(entity => entity.Id)
                        .UseSerialColumn();

        modelBuilder.Entity<GameStatsReadModel>()
            .Property(entity => entity.TeamId)
            .HasColumnType("Uuid");

        modelBuilder.Entity<GameStatsReadModel>()
            .Property(entity => entity.GameId)
            .HasColumnType("Uuid");

        modelBuilder.Entity<GameStatsReadModel>()
            .Property(entity => entity.PlayerId)
            .HasColumnType("Uuid");

        modelBuilder.Entity<GameStatsReadModel>()
            .Property(entity => entity.Blocks)
            .HasColumnType("smallint");

        modelBuilder.Entity<GameStatsReadModel>()
            .Property(entity => entity.BlocksReceived)
            .HasColumnType("smallint");

        modelBuilder.Entity<GameStatsReadModel>()
            .Property(entity => entity.Fouls)
            .HasColumnType("smallint");

        modelBuilder.Entity<GameStatsReadModel>()
            .Property(entity => entity.FoulsProvoked)
            .HasColumnType("smallint");

        modelBuilder.Entity<GameStatsReadModel>()
            .Property(entity => entity.DefensiveRebounds)
            .HasColumnType("smallint");

        modelBuilder.Entity<GameStatsReadModel>()
            .Property(entity => entity.OffensiveRebounds)
            .HasColumnType("smallint");

        modelBuilder.Entity<GameStatsReadModel>()
            .Property(entity => entity.Steals)
            .HasColumnType("smallint");

        modelBuilder.Entity<GameStatsReadModel>()
            .Property(entity => entity.Turnovers)
            .HasColumnType("smallint");

        modelBuilder.Entity<GameStatsReadModel>()
            .Property(entity => entity.MadeTwoPoints)
            .HasColumnType("smallint");

        modelBuilder.Entity<GameStatsReadModel>()
            .Property(entity => entity.MadeFreeThrows)
            .HasColumnType("smallint");

        modelBuilder.Entity<GameStatsReadModel>()
            .Property(entity => entity.MadeThreePoints)
            .HasColumnType("smallint");

        modelBuilder.Entity<GameStatsReadModel>()
            .Property(entity => entity.MissedFreeThrows)
            .HasColumnType("smallint");

        modelBuilder.Entity<GameStatsReadModel>()
            .Property(entity => entity.MissedThreePoints)
            .HasColumnType("smallint");

        modelBuilder.Entity<GameStatsReadModel>()
            .Property(entity => entity.MissedTwoPoints)
            .HasColumnType("smallint");
    }

    private static void DataModelsIndexesBuilder(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Domain.Entities.Stream>()
            .HasIndex(entity => new { entity.StreamId });

        modelBuilder.Entity<Domain.Entities.Stream>()
            .HasIndex(entity => entity.EventId)
            .IsUnique();

        modelBuilder.Entity<Game>()
            .HasIndex(entity => entity.GameId);
    }

    private static void TeamDataModelBuilder(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Team>()
                        .Property(entity => entity.Id)
                        .UseSerialColumn();

        modelBuilder.Entity<Team>()
            .Property(entity => entity.TeamId)
            .HasColumnType("Uuid");

        modelBuilder.Entity<Team>()
            .Property(entity => entity.IsActive)
            .HasDefaultValue(true);

        modelBuilder.Entity<Team>()
            .Property(entity => entity.Founded)
            .HasColumnType("date");

        modelBuilder.Entity<Team>()
            .Property(entity => entity.Name)
            .HasColumnType("Varchar")
            .HasMaxLength(255);
    }

    private static void ArenaDataModelBuilder(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Arena>()
                        .Property(entity => entity.Id)
                        .UseSerialColumn();

        modelBuilder.Entity<Arena>()
            .Property(entity => entity.ArenaId)
            .HasColumnType("Uuid");

        modelBuilder.Entity<Arena>()
            .Property(entity => entity.Address)
            .HasColumnType("Varchar")
            .HasMaxLength(255);

        modelBuilder.Entity<Arena>()
            .Property(entity => entity.Name)
            .HasColumnType("Varchar")
            .HasMaxLength(255);

        modelBuilder.Entity<Arena>()
            .Property(entity => entity.Built)
            .HasColumnType("date");

        modelBuilder.Entity<Arena>()
            .Property(entity => entity.Capacity)
            .HasColumnType("Integer");

        modelBuilder.Entity<Arena>()
            .Property(entity => entity.Address)
            .HasColumnType("Varchar")
            .HasMaxLength(255);
    }

    private static void GamDataModelBuilder(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Game>()
            .Property(entity => entity.Id)
            .UseSerialColumn();

        modelBuilder.Entity<Game>()
            .Property(entity => entity.GameId)
            .HasColumnType("Uuid");

        modelBuilder.Entity<Game>()
            .Property(entity => entity.TeamAwayId)
            .HasColumnType("Uuid");

        modelBuilder.Entity<Game>()
            .Property(entity => entity.TeamHomeId)
            .HasColumnType("Uuid");

        modelBuilder.Entity<Game>()
            .Property(entity => entity.GameTime)
            .HasColumnType("TimestampTz");
    }

    private static void StreamDataModelBuilder(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Domain.Entities.Stream>()
                        .Property(entity => entity.Id)
                        .UseSerialColumn();

        modelBuilder.Entity<Domain.Entities.Stream>()
            .Property(entity => entity.StreamId)
            .HasColumnType("Uuid");

        modelBuilder.Entity<Domain.Entities.Stream>()
            .Property(entity => entity.EventId)
            .HasColumnType("Uuid");

        modelBuilder.Entity<Domain.Entities.Stream>()
            .Property(entity => entity.Type)
            .HasColumnType("Varchar")
            .HasMaxLength(255);

        modelBuilder.Entity<Domain.Entities.Stream>()
            .Property(entity => entity.Data)
            .HasColumnType("Text");

        modelBuilder.Entity<Domain.Entities.Stream>()
            .Property(entity => entity.MetaData)
            .HasColumnType("Varchar");

        modelBuilder.Entity<Domain.Entities.Stream>()
            .Property(entity => entity.CreatedAt)
            .HasColumnType("TimestampTz");
    }
}
