namespace VFXChallenge.Infrastructure.Repositories
{
    using Domain.ForeignExchanges;
    public interface IForeignExchangeRatesRepository
    {
        Task<ForeignExchangeRate> GetAsync(string fromCurrency, string toCurrency);
        
        Task<ForeignExchangeRate> GetAsync(Guid id);

        Task<ForeignExchangeRate> CreateAsync(ForeignExchangeRate rate);

        Task UpdateAsync(Guid id, ForeignExchangeRate rate);

        Task DeleteAsync(ForeignExchangeRate rate);

        Task<bool> ExistsAsync(Guid id);
    }
}