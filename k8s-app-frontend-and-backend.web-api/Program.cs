using k8s_app_frontend_and_backend.web_api.Configuration;
using k8s_app_frontend_and_backend.web_api.Repositories;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Diagnostics.HealthChecks;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddHealthChecks()
    // Liveness: Just checks if the app process is running. No dependencies.
    .AddCheck("self", () => HealthCheckResult.Healthy(), tags: new[] { "live" })
    .AddSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection"),
        name: "database-check",
        tags: new[] { "ready" }); // Notice the "ready" tag

// Add services to the container.
builder.Services.AddControllers();

builder.Services.AddDbContext<WeatherDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Configure WeatherApi settings from appsettings.json or environment variables (K8s ConfigMaps)
builder.Services.Configure<WeatherApiConfig>(
    builder.Configuration.GetSection(WeatherApiConfig.SectionName));

// Add CORS configuration
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAngularDev",
        policy =>
        {
 policy.AllowAnyOrigin()
      .AllowAnyMethod()
    .AllowAnyHeader();
        });
});

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Map Liveness endpoint
app.MapHealthChecks("/health/live", new HealthCheckOptions
{
  Predicate = check => check.Tags.Contains("live")
});

// Map Readiness endpoint
app.MapHealthChecks("/health/ready", new HealthCheckOptions
{
  Predicate = check => check.Tags.Contains("ready")
});

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Use CORS - must be before UseHttpsRedirection
app.UseCors("AllowAngularDev");

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
