using horseapispring26.Repositories;
using horseapispring26.Data;
using horseapispring26.Services;
using horseapispring26.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authentication;

var builder = WebApplication.CreateBuilder(args);
var authEnabled = builder.Configuration.GetValue<bool>("Auth:Enabled");
var sqlEnabled = builder.Configuration.GetValue<bool>("DataSources:SqlServer:Enabled");
var jsonEnabled = builder.Configuration.GetValue<bool>("DataSources:JsonFile:Enabled");

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddProblemDetails();
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

// Configure CORS for MAUI application
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowMAUI", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

if (sqlEnabled)
{
    var noAuthConnection = builder.Configuration.GetConnectionString("HorseNoAuthConnection")
                           ?? builder.Configuration.GetConnectionString("DefaultConnection")
                           ?? throw new InvalidOperationException("ConnectionStrings:HorseNoAuthConnection (or DefaultConnection) is required when SQL is enabled.");

    var authConnection = builder.Configuration.GetConnectionString("HorseAuthConnection")
                         ?? builder.Configuration.GetConnectionString("DefaultConnection")
                         ?? throw new InvalidOperationException("ConnectionStrings:HorseAuthConnection (or DefaultConnection) is required when SQL is enabled.");

    if (authEnabled)
    {
        builder.Services.AddDbContext<AuthHorseDbContext>(options =>
        {
            options.UseSqlServer(authConnection);
            if (builder.Environment.IsDevelopment())
            {
                options.EnableSensitiveDataLogging();
                options.EnableDetailedErrors();
            }
        });

        builder.Services.AddScoped<IHorseDataContext>(sp => sp.GetRequiredService<AuthHorseDbContext>());
    }
    else
    {
        builder.Services.AddDbContext<HorseDbContext>(options =>
        {
            options.UseSqlServer(noAuthConnection);
            if (builder.Environment.IsDevelopment())
            {
                options.EnableSensitiveDataLogging();
                options.EnableDetailedErrors();
            }
        });

        builder.Services.AddScoped<IHorseDataContext>(sp => sp.GetRequiredService<HorseDbContext>());
    }
}

if (authEnabled)
{
    if (!sqlEnabled)
    {
        throw new InvalidOperationException("Auth mode requires SQL Server enabled.");
    }

    // Identity API endpoints (/register, /login, /refresh, etc.)
    builder.Services
        .AddIdentityApiEndpoints<IdentityUser>(options =>
        {
            options.User.RequireUniqueEmail = true;
        })
        .AddRoles<IdentityRole>()
        .AddEntityFrameworkStores<AuthHorseDbContext>();
}
else
{
    // Demo mode: all [Authorize] endpoints authenticate automatically.
    builder.Services
        .AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = DemoAuthenticationHandler.SchemeName;
            options.DefaultChallengeScheme = DemoAuthenticationHandler.SchemeName;
        })
        .AddScheme<AuthenticationSchemeOptions, DemoAuthenticationHandler>(
            DemoAuthenticationHandler.SchemeName,
            _ => { });
}

builder.Services.AddAuthorization();

// Register repository and data sources based on configuration
if (jsonEnabled)
{
    builder.Services.AddScoped<IHorseRepository, JsonHorseRepository>();
}
else
{
    if (!sqlEnabled)
    {
        throw new InvalidOperationException("SQL horse repository selected but DataSources:SqlServer:Enabled is false.");
    }

    builder.Services.AddScoped<IHorseRepository, SqlHorseRepository>();
}

builder.Services.AddSingleton(TimeProvider.System);
builder.Services.AddScoped<IHorseMapper, HorseMapper>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseExceptionHandler();

app.UseHttpsRedirection();

// Enable CORS
app.UseCors("AllowMAUI");

// Authentication must come BEFORE Authorization
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
if (authEnabled)
{
    app.MapIdentityApi<IdentityUser>();
}

// Apply migrations and optional JSON import in Development
if (app.Environment.IsDevelopment() && sqlEnabled)
{
    await DbInitializer.InitializeAsync(app.Services, app.Configuration, authEnabled);
}

app.Run();
