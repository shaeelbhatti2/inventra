using Inventra.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Inventra.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("Default")
            ?? configuration["CONNECTION_STRING"]
            ?? "Host=localhost;Port=5433;Database=inventra;Username=inventra;Password=inventra_dev";

        services.AddDbContext<InventraDbContext>(options =>
            options.UseNpgsql(connectionString));

        return services;
    }
}
