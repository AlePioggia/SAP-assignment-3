using Ocelot.Provider.Consul;
using Ocelot.DependencyInjection;
using Ocelot.Middleware;
using ApiGateway;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration.AddJsonFile("ocelot.json", optional: false, reloadOnChange: true);
builder.Services.AddOcelot(builder.Configuration).AddConsul<MyConsulServiceBuilder>();
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

var app = builder.Build();

app.UseCors();

await app.UseOcelot();

app.MapHealthChecks("/health");

app.Run();