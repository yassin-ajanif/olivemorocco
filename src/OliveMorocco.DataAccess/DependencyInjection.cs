using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using OliveMorocco.DataAccess.Repositories;

namespace OliveMorocco.DataAccess;

public static class DependencyInjection
{
    public static IServiceCollection AddDataAccess(
        this IServiceCollection services,
        string connectionString)
    {
        services.AddDbContext<AppDbContext>(options =>
            options.UseNpgsql(connectionString));

        services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
        services.AddScoped<IAppDatabaseInitializer, DatabaseInitializer>();

        return services;
    }
}
