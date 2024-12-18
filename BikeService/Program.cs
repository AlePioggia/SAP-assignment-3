using BikeService.application;
using BikeService.controller;
using BikeService.infrastructure;
using Consul;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSignalR();

builder.Services.AddSingleton<IBikeService, BikeService.application.BikeService>();
builder.Services.AddSingleton<IBikeRepository, BikeRepository>();

builder.Services.AddScoped<IPositionNotifier, SignalRPositionNotifier>();

builder.Services.AddSingleton<IConsulClient, ConsulClient>(sp =>
{
    var consulAddress = builder.Configuration["Consul:Address"] ?? "http://consul:8500";
    return new ConsulClient(ConfigurationBinder => ConfigurationBinder.Address = new Uri(consulAddress));
});

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

public class ConsulRegistrationService : IHostedService
{
    private readonly IConsulClient _consulClient;
    private readonly string _serviceId;

    public ConsulRegistrationService(IConsulClient consulClient)
    {
        _consulClient = consulClient;
        _serviceId = Guid.NewGuid().ToString();
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        var registration = new AgentServiceRegistration()
        {
            ID = _serviceId,
            Name = "bike-service",
            Address = "bike-service",
            Port = 8080,
            Tags = new[] { "bike", "api" }
        };

        await _consulClient.Agent.ServiceRegister(registration);
    }

    public async Task StopAsync(CancellationToken cancellationToken)
    {
        await _consulClient.Agent.ServiceDeregister(_serviceId);
    }
}