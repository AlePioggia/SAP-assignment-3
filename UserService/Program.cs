using Consul;
using MongoDB.Driver;
using StackExchange.Redis;
using UserService.application;
using UserService.infrastructure;
using UserService.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSingleton<UserService.application.IUserService, UserService.application.UserService>();
builder.Services.AddSingleton<UserService.application.ISessionService, RedisSessionService>();

builder.Services.AddSingleton<IUserRepository, UserRepository>();

builder.Services.AddSingleton<IConnectionMultiplexer>(sp =>
{
    var configuration = builder.Configuration.GetConnectionString("Redis");
    var redisConnectionString = configuration + ",abortConnect=false";
    return ConnectionMultiplexer.Connect(configuration);
});

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

builder.Services.AddHealthChecks()
    .AddCheck<CustomHealthCheck>("Custom health check", tags: new[] { "critical" }); ;

var app = builder.Build();

app.UseCors(options =>
{
    options.AllowAnyOrigin();
    options.AllowAnyMethod();
    options.AllowAnyHeader();
});

app.MapControllers();
app.MapHealthChecks("/health");
app.Run();