namespace VFXChallenge.Application.Abstractions
{
    using Domain.ForeignExchanges;

    using DTO;
    public interface ICurrencyExchangeRateService
    {
        Task<ForeignExchangeRateResponse> GetAsync(string fromCurrency, string toCurrency);

        Task<ForeignExchangeRateResponse> CreateAsync(CreateForeignExchangeRate request);

        Task UpdateAsync(UpdateForeignExchangeRate request, Guid id);

        Task DeleteAsync(Guid id);
    }
}