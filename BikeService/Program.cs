using BikeService.application;
using BikeService.controller;
using BikeService.infrastructure;
using Consul;
using EventStore.Client;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using RabbitMQ.Client;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSignalR();

builder.Services.AddSingleton<IBikeService, BikeService.application.BikeService>();
builder.Services.AddSingleton<IBikeRepository, BikeRepository>();
builder.Services.AddHostedService<RabbitMqConsumer>();
builder.Services.AddSingleton<IPositionNotifier, SignalRPositionNotifier>();

builder.Services.AddSingleton<IConsulClient, ConsulClient>(sp =>
{
    var consulAddress = builder.Configuration["Consul:Address"] ?? "http://consul-agent:8500";
    return new ConsulClient(ConfigurationBinder => ConfigurationBinder.Address = new Uri(consulAddress));
});

builder.Services.AddSingleton(sp =>
{
    var settings = EventStoreClientSettings.Create("esdb://eventstore:2113?tls=false");
    return new EventStoreClient(settings);
});

builder.Services.AddControllers();

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.Authority = "https://your-identity-server";
        options.RequireHttpsMetadata = false;
        options.Audience = "YourApiAudience";
    });

builder.Services.AddAuthentication();
builder.Services.AddAuthorization();

builder.Services.AddControllers();

var app = builder.Build();

app.UseCors(options =>
{
    options.WithOrigins("http://localhost:4200")
        .AllowAnyMethod()
        .AllowAnyHeader()
        .AllowCredentials();
});

app.MapHub<BikeHub>("/bikeHub");

app.MapControllers();
//app.MapHealthChecks("/health");
app.Run();