namespace VFXChallenge.Infrastructure
{
    using AlphaVantageApi;

    using Messaging;

    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;

    using Repositories;

    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            var connectionString = configuration.GetConnectionString("SQLServer") ?? 
                                   throw new ArgumentNullException(nameof(configuration));

            services.AddDbContext<ExchangeRatesDbContext>(options =>
            {
                options.UseSqlServer(connectionString);
            });

            services.AddScoped<IAlphaVantageGateway, AlphaVantageGateway>();
            services.AddScoped<IForeignExchangeRatesRepository, ForeignExchangeRatesRepository>();
            services.AddScoped<IProduceForeignExchangeRateCreatedMessage, ProduceForeignExchangeRateCreatedMessage>();
            
            return services;
        }
    }
}