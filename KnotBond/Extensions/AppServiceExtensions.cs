using KnotBond.Data;
using KnotBond.interfaces;
using KnotBond.Services;
using Microsoft.EntityFrameworkCore;

namespace KnotBond.Extensions;

public static class AppServiceExtensions
{
    public static IServiceCollection AddAppServices(this IServiceCollection services
        , IConfiguration configuration)
    {
        services.AddDbContext<DataContext>(opt =>
        {
            opt.UseSqlite(configuration.GetConnectionString("DefaultConnection"));
        });
        services.AddCors();
        services.AddScoped<ITokenService, TokenService>();
        return services;
    }
}
