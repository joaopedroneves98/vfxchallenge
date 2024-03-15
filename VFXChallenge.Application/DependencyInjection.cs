namespace VFXChallenge.Application
{
    using Abstractions;

    using DTO;

    using FluentValidation;

    using Microsoft.Extensions.DependencyInjection;

    using Services;

    using Validators;
    public static class DependencyInjection
    {
        public static IServiceCollection AddApplication(this IServiceCollection services)
        {
            services.AddScoped<ICurrencyExchangeRateService, CurrencyExchangeRateService>();
            
            services.AddScoped<IValidator<CreateForeignExchangeRate>, CreateForeignExchangeRateValidator>();
            services.AddScoped<IValidator<UpdateForeignExchangeRate>, UpdateForeignExchangeRateValidator>();
            ValidatorOptions.Global.LanguageManager.Enabled = false;
            
            return services;
        }
    }
}