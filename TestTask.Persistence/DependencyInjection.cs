using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using TestTask.Application.Interfaces;

namespace TestTask.Persistence
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddPersistence(this  IServiceCollection services, IConfiguration configuration)
        {
            var connectionString = configuration["ConnectionString"];
            services.AddDbContext<TestDbContext>(options =>
            {
                options.UseSqlServer(connectionString);
            });

            services.AddScoped<ITestDbContext>(provider => provider.GetService<TestDbContext>());
            return services;
        }
    }
}
