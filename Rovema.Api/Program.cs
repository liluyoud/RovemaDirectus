using Dclt.Shared.Helpers;
using Rovema.Api;
using Rovema.Shared.Services;

var builder = WebApplication.CreateBuilder(args);

// environment variables
var showSwagger = Environment.GetEnvironmentVariable("SHOW_SWAGGER") ?? builder.Configuration["Environment:SHOW_SWAGGER"];

// swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// services
builder.Services.AddDcltServices(builder.Configuration);
builder.Services.AddSingleton<ReadService>();


// app
var app = builder.Build();

if (app.Environment.IsDevelopment() || showSwagger == "true")
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.MapRpasEndpoints();
app.Run();

