using Consul;
using MongoDB.Driver;
using RentalService.application;
using RentalService.application.ride;
using RentalService.infrastructure;
using RentalService.infrastructure.ride;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSingleton<IRideService, RideService>();
builder.Services.AddSingleton<IRideRepository, RideRepository>();

builder.Services.AddSingleton<IEventPublisher, RabbitMQEventPublisher>();

builder.Services.AddSingleton<IMongoClient>(sp =>
{
    var mongoDbConnectionString = builder.Configuration.GetConnectionString("MongoDB");
    return new MongoClient(mongoDbConnectionString);
});

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
            Name = "rental-service",
            Address = "rental-service",
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