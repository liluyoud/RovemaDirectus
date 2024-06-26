using Microsoft.EntityFrameworkCore;
using Coravel.Pro;
using Rovema.Coravel.Data;
using Rovema.Shared.Services;
using Dclt.Services.OpenWeather;
using Dclt.Directus;


var builder = WebApplication.CreateBuilder(args);

// environment variables
var rovemaSqlite = builder.Configuration["Environment:ROVEMA_SQLITE"];
var rovemaApiUrl = builder.Configuration["Environment:ROVEMAAPI_URL"];

// services
builder.Services.AddDbContext<CoravelContext>(options =>  options.UseSqlite(rovemaSqlite));
builder.Services.AddDatabaseDeveloperPageExceptionFilter();
builder.Services.AddRazorPages().AddNewtonsoftJson(); 
builder.Services.AddCoravelPro(typeof(CoravelContext));
builder.Services.AddHttpClient();

builder.Services.AddScoped<OpenWeatherService>();
builder.Services.AddScoped<DirectusService>();
builder.Services.AddScoped<ReadService>();
builder.Services.AddSingleton<PlayService>();
//builder.Services.AddRefit<IRovemaService>(rovemaApiUrl!);


var app = builder.Build();
app.ApplyMigrations();
if (app.Environment.IsProduction()) { 
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseStaticFiles();
app.UseRouting();
app.UseAuthorization();
app.MapRazorPages();
app.UseCoravelPro();
app.Run();
