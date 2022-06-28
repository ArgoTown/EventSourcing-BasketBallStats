using BasketballStats.Api.Application.CommandHandlers;
using BasketballStats.Api.Application.Queries;
using BasketballStats.Api.Application.QueryHandlers;
using BasketballStats.Domain.Models;
using BasketballStats.Domain.Repositories;
using BasketballStats.Domain.Services;
using BasketballStats.Infrastructure.Repositories;
using BasketballStats.Infrastructure.Synchronizer;
using Microsoft.EntityFrameworkCore;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers().AddJsonOptions(options => options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter()));
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services
    .AddEndpointsApiExplorer()
    .AddSwaggerGen()
    .AddSingleton<ITypeResolverService, TypeResolverService>()
    .AddScoped<IEventsService, EventsService>()
    .AddScoped(sp => new StatisticCommandHandler(
        sp.GetRequiredService<IEventStoreRepository>(),
        sp.GetRequiredService<IEventsService>()
        ))
    .AddScoped<IQueryHandler<GetPlayerStatisticsQuery, PlayerStats>, PlayerStatisticsQueryHandler>()
    .AddScoped<ICommandHandler, StatisticCommandHandler>();

var isDataInMemory = builder.Configuration.GetRequiredSection("Database").GetValue<bool>("InMemory");
if (isDataInMemory)
{
    builder.Services
        .AddSingleton<IEventStoreRepository, EventStoreInMemoryRepository>()
        .AddSingleton<IGameRepository, GameInMemoryRepository>()
        .AddSingleton<IGameStatsReadModel, GameStatsInMemoryReadModelRepository>()
        .AddHostedService<ReadModelInMemoryBackgroundService>();
}
else
{
    builder.Services
        .AddScoped<IEventStoreRepository, EventStoreRepository>()
        .AddScoped<IGameRepository, GameRepository>()
        .AddDbContextPool<EventsContext>(options => options.UseNpgsql("Host=postgres;Database=BasketBallStats;Username=guest;Password=guest"))
        .AddScoped<IGameStatsReadModel, GameStatsReadModelRepository>()
        .AddHostedService<ReadModelBackgroundService>();
}

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

if (!isDataInMemory)
{
    using var scope = app.Services.CreateScope();
    var db = scope.ServiceProvider.GetRequiredService<EventsContext>();
    db.Database.Migrate();
}

await app.RunAsync();
