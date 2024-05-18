using Dclt.Shared.Extensions;
using Dclt.Shared.Helpers;
using Rovema.Api;
using Rovema.Shared.Interfaces;
using Rovema.Shared.Services;

var builder = WebApplication.CreateBuilder(args);

// environment variables
var directusUrl = Environment.GetEnvironmentVariable("DIRECTUS_URL") ?? builder.Configuration["Environment:DIRECTUS_URL"];
var redisUrl = Environment.GetEnvironmentVariable("REDIS_URL") ?? builder.Configuration["Environment:REDIS_URL"];
var showSwagger = Environment.GetEnvironmentVariable("SHOW_SWAGGER") ?? builder.Configuration["Environment:SHOW_SWAGGER"];

// swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// redis
builder.Services.AddStackExchangeRedisCache(options => options.Configuration = redisUrl);

// services
builder.Services.AddDcltServices();
builder.Services.AddRefit<IDirectusService>(directusUrl!);

builder.Services.AddScoped<RovemaService>();

// app
var app = builder.Build();

if (showSwagger == "true")
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.MapRpaEndpoints();
app.Run();

