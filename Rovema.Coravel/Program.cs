using Coravel.Pro;
using Dclt.Shared.Helpers;
using Microsoft.EntityFrameworkCore;
using Rovema.Coravel.Data;

var builder = WebApplication.CreateBuilder(args);

// environment variables
var directusUrl = Environment.GetEnvironmentVariable("DIRECTUS_URL") ?? builder.Configuration["Environment:DIRECTUS_URL"];
var accessToken = Environment.GetEnvironmentVariable("DIRECTUS_TOKEN") ?? builder.Configuration["Environment:DIRECTUS_TOKEN"] ?? "";
var rovemaSqlite = builder.Configuration["Environment:ROVEMA_SQLITE"];
var rovemaApiUrl = builder.Configuration["Environment:ROVEMAAPI_URL"];

builder.Services.AddDbContext<CoravelContext>(options =>  options.UseSqlite(rovemaSqlite));
builder.Services.AddDatabaseDeveloperPageExceptionFilter();
builder.Services.AddRazorPages().AddNewtonsoftJson(); 
builder.Services.AddCoravelPro(typeof(CoravelContext));
builder.Services.AddDcltServices(directusUrl!, accessToken!);

var app = builder.Build();
app.ApplyMigrations();
if (app.Environment.IsProduction()) { 
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthorization();
app.MapRazorPages();
app.UseCoravelPro();
app.Run();
