using Dclt.Directus;
using Rovema.Api;
using Rovema.Shared.Services;

var builder = WebApplication.CreateBuilder(args);

// environment variables
var showSwagger = Environment.GetEnvironmentVariable("SHOW_SWAGGER") ?? builder.Configuration["Environment:SHOW_SWAGGER"] ?? "false";

// swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// cors
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.AllowAnyHeader();
        policy.AllowAnyOrigin();
        policy.AllowAnyMethod();
    });
});

// services
builder.Services.AddHttpClient();
builder.Services.AddScoped<DirectusService>();
builder.Services.AddScoped<ReadService>();

// app
var app = builder.Build();

if (app.Environment.IsDevelopment() || showSwagger == "true")
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors();
app.MapRpasEndpoints();
app.Run();

