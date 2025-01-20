using Consul;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using MongoDB.Driver;
using StationService.application;
using StationService.infrastructure;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSingleton<IStationService, StationService.application.StationService>();
builder.Services.AddSingleton<IStationRepository, StationRepository>();
builder.Services.AddHostedService<RabbitMqConsumer>();
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

var app = builder.Build();
app.MapControllers();
//app.MapHealthChecks("/health");
app.Run();