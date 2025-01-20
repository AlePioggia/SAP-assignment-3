using Consul;
using Microsoft.AspNetCore.Authentication.JwtBearer;
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

app.UseCors(options =>
{
    options.WithOrigins("http://localhost:4200")
        .AllowAnyMethod()
        .AllowAnyHeader()
        .AllowCredentials();
});

app.MapControllers();
app.Run();