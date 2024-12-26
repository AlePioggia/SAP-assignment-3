using Consul;
using SmartCityService.application;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSingleton<IBikeRepository, BikeRepository>();
builder.Services.AddSingleton<IStationRepository, StationRepository>();
builder.Services.AddSingleton<ISmartCityService, SmartCityService>();

builder.Services.AddSingleton<IConsulClient, ConsulClient>(sp =>
{
    var consulAddress = builder.Configuration["Consul:Address"] ?? "http://consul-agent:8500";
    return new ConsulClient(config => config.Address = new Uri(consulAddress));
});

builder.Services.AddSingleton<IHostedService, ConsulRegistrationService>();

builder.Services.AddControllers();

var app = builder.Build();

app.UseCors(options =>
{
    options.WithOrigins("http://localhost:4200")
        .AllowAnyMethod()
        .AllowAnyHeader()
        .AllowCredentials();
});

app.MapControllers();
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
            Name = "smart-city-service",
            Address = "smart-city-service",
            Port = 8080,
            Tags = new[] { "smart", "api" }
        };

        await _consulClient.Agent.ServiceRegister(registration);
    }

    public async Task StopAsync(CancellationToken cancellationToken)
    {
        await _consulClient.Agent.ServiceDeregister(_serviceId);
    }
}