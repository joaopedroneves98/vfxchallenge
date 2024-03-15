namespace VFXChallenge.Infrastructure.AlphaVantageApi
{
    using AlphaVantage.Net.Common.Currencies;
    using AlphaVantage.Net.Core.Client;
    using AlphaVantage.Net.Forex.Client;

    using Configurations;

    using Domain.ForeignExchanges;

    using Microsoft.Extensions.Logging;
    using Microsoft.Extensions.Options;

    public class AlphaVantageGateway : IAlphaVantageGateway
    {
        private readonly AlphaVantageClient _client;
        private readonly ILogger<AlphaVantageGateway> _logger;
        
        public AlphaVantageGateway(IOptions<AlphaVantageApi> options,
            ILogger<AlphaVantageGateway> logger)
        {
            this._logger = logger;
            this._client = new AlphaVantageClient(options.Value.APIKey);
        }

        /// <summary>
        /// Requests the AlphaVantage API for the given currency pair
        /// </summary>
        /// <param name="fromCurrency">The from currency code</param>
        /// <param name="toCurrency">The to currency code</param>
        /// <returns></returns>
        /// <exception cref="ArgumentException">Can be thrown if the given currencies are not found in the enum PhysicalCurrency</exception>
        public async Task<ForeignExchangeRate> GetExchangeRateForCurrencyPairAsync(string fromCurrency, string toCurrency)
        {
            // Opted to use a third party library as it had already created a client for the API and DTOs
            if (!Enum.TryParse<PhysicalCurrency>(fromCurrency, true, out var fromCurrencyEnum) || 
                !Enum.TryParse<PhysicalCurrency>(toCurrency, true, out var toCurrencyEnum))
            {
                throw new ArgumentException("Invalid Currency Codes");
            }

            try
            {
                var response = await this._client.Forex()
                    .GetExchangeRateAsync(fromCurrencyEnum, toCurrencyEnum);

                return new ForeignExchangeRate(
                    fromCurrency,
                    toCurrency,
                    response.BidPrice, 
                    response.AskPrice, 
                    response.FromCurrencyName, 
                    response.ToCurrencyName);
            }
            catch (Exception e)
            {
                this._logger.LogError("Error communicating with the external API.");
                
                throw;
            }
        }
    }
}