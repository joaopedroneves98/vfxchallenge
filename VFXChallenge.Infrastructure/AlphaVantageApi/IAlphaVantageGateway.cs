namespace VFXChallenge.Infrastructure.AlphaVantageApi
{
    using Domain.ForeignExchanges;
    public interface IAlphaVantageGateway
    {
        Task<ForeignExchangeRate> GetExchangeRateForCurrencyPairAsync(string fromCurrency, string toCurrency);
    }
}