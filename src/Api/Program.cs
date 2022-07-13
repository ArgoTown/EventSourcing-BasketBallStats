using BasketballStats.Api.Application.CommandHandlers;
using BasketballStats.Api.Application.Queries;
using BasketballStats.Api.Application.QueryHandlers;
using BasketballStats.Domain.Models;
using BasketballStats.Domain.Repositories;
using BasketballStats.Domain.Services;
using BasketballStats.Infrastructure.Repositories;
using BasketballStats.Infrastructure.Synchronizer;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using System.Reflection;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers().AddJsonOptions(options => options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter()));

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services
    .AddEndpointsApiExplorer()
    .AddSwaggerGen(options =>
    {
        options.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, $"{Assembly.GetExecutingAssembly().GetName().Name}.xml"));
        options.SwaggerDoc("v1", new OpenApiInfo
        {
            Version = "v1",
            Title = "Basketball Stats API",
            Description = "Event sourcing API Example"
        });
    })
    .AddSingleton<ITypeResolverService, TypeResolverService>()
    .AddScoped<IEventsService, EventsService>()
    .AddScoped<IQueryHandler<GetPlayerStatisticsQuery, PlayerStats>, PlayerStatisticsQueryHandler>()
    .AddScoped<ICommandHandler, StatisticCommandHandler>();

var healthChecks = builder.Services.AddHealthChecks();

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
    healthChecks.AddNpgSql(builder.Configuration["Database:ConnectionStrings:PostgreSql"], tags: new[] { "sql", "ready" });

    builder.Services
        .AddScoped<IEventStoreRepository, EventStoreRepository>()
        .AddScoped<IGameRepository, GameRepository>()
        .AddDbContextPool<EventsContext>(options => options.UseNpgsql(builder.Configuration["Database:ConnectionStrings:PostgreSql"]))
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
    app.MapHealthChecks("/health/ready", new HealthCheckOptions { Predicate = healthCheck => healthCheck.Tags.Contains("ready") });
}

app.MapHealthChecks("/health/live", new HealthCheckOptions { Predicate = healthCheck => false });

await app.RunAsync();
