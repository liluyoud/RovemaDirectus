using Coravel.Pro;
using Dclt.Shared.Extensions;
using Microsoft.EntityFrameworkCore;
using Rovema.Coravel.Data;
using Rovema.Shared.Interfaces;
using Rovema.Shared.Services;

var builder = WebApplication.CreateBuilder(args);

// environment variables
var rovemaSqlite = builder.Configuration["Environment:ROVEMA_SQLITE"];
var rovemaApiUrl = builder.Configuration["Environment:ROVEMAAPI_URL"];

builder.Services.AddDbContext<CoravelContext>(options =>  options.UseSqlite(rovemaSqlite));
builder.Services.AddDatabaseDeveloperPageExceptionFilter();
builder.Services.AddRazorPages().AddNewtonsoftJson(); 
builder.Services.AddCoravelPro(typeof(CoravelContext));
builder.Services.AddRefit<IRovemaService>(rovemaApiUrl!);

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
