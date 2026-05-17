using k8s_app_frontend_and_backend.web_api.Models;
using Microsoft.EntityFrameworkCore;

namespace k8s_app_frontend_and_backend.web_api.Repositories
{
  public class WeatherDbContext : DbContext
  {
    public WeatherDbContext(DbContextOptions<WeatherDbContext> options) : base(options) { }
    public DbSet<WeatherForecast> WeatherForecasts { get; set; }
  }
}
