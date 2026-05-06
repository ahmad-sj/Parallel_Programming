using Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using TanvirArjel.EFCore.GenericRepository;

namespace Infrastructure
{
    public static class DependecyInjection
    {

        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDatabase(configuration);
            services.AddAppRepository();
            return services;
        }

        private static IServiceCollection AddDatabase(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<EcomDbContext>(options => options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));
                //.AddIdentity<User, IdentityRole>();
            return services;
        }

        private static IServiceCollection AddAppRepository(this IServiceCollection services)
        {
            services.AddGenericRepository<EcomDbContext>();
            return services;
        }
    }
}
