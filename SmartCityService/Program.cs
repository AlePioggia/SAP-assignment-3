using Consul;
using SmartCityService.application;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSingleton<IBikeRepository, BikeRepository>();
builder.Services.AddSingleton<IStationRepository, StationRepository>();
builder.Services.AddSingleton<ISmartCityService, SmartCityService.application.SmartCityService>();

builder.Services.AddSingleton<IConsulClient, ConsulClient>(sp =>
{
    var consulAddress = builder.Configuration["Consul:Address"] ?? "http://consul-agent:8500";
    return new ConsulClient(config => config.Address = new Uri(consulAddress));
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

app.MapControllers();
app.Run();