namespace VFXChallenge.Infrastructure.Repositories
{
    using Domain.ForeignExchanges;

    using Microsoft.EntityFrameworkCore;
    public class ForeignExchangeRatesRepository : IForeignExchangeRatesRepository
    {
        private readonly ExchangeRatesDbContext _dbContext;

        public ForeignExchangeRatesRepository(ExchangeRatesDbContext dbContext)
        {
            this._dbContext = dbContext;
        }

        /// <summary>
        /// Gets a single Foreign exchange rate from the database, matching the given parameters
        /// </summary>
        /// <param name="fromCurrency">The from currency code</param>
        /// <param name="toCurrency">The to currency code</param>
        /// <returns>The existing foreign exchange rate, if found, or null if not found</returns>
        public async Task<ForeignExchangeRate> GetAsync(string fromCurrency, string toCurrency)
        {
            // possible null value will be handled by the calling service
            return await this._dbContext.ForeignExchangeRates
                .FirstOrDefaultAsync(x => x.ToCurrencyCode == toCurrency && x.FromCurrencyCode == fromCurrency);
        }
        
        /// <summary>
        /// Gets a single Foreign exchange rate from the database, with the given id
        /// </summary>
        /// <param name="id">The id to search for</param>
        /// <returns>The existing foreign exchange rate, if found, or null if not found</returns>
        public async Task<ForeignExchangeRate> GetAsync(Guid id)
        {
            // possible null value will be handled by the calling service
            return await this._dbContext.ForeignExchangeRates.FindAsync(id);
        }
        
        /// <summary>
        /// Creates the foreign exchange rate in the database
        /// </summary>
        /// <param name="rate">The rate to be created</param>
        /// <returns>The created rate</returns>
        public async Task<ForeignExchangeRate> CreateAsync(ForeignExchangeRate rate)
        {
            await this._dbContext.ForeignExchangeRates.AddAsync(rate);

            await this._dbContext.SaveChangesAsync();

            return rate;
        }
        
        /// <summary>
        /// Updates an existing foreign exchange rate in the database
        /// </summary>
        /// <param name="id">The id of the rate to be updated</param>
        /// <param name="rate">The values to update the rate with</param>
        /// <exception cref="Exception"></exception>
        public async Task UpdateAsync(Guid id, ForeignExchangeRate rate)
        {
            var existingRate = await this.GetAsync(id);
            if (existingRate is null)
            {
                throw new Exception("Rate not found");
            }
            
            existingRate.SetAsk(rate.Ask);
            existingRate.SetBid(rate.Bid);
            existingRate.SetFromCurrencyCode(rate.FromCurrencyCode);
            existingRate.SetFromCurrencyName(rate.FromCurrencyName);
            existingRate.SetToCurrencyCode(rate.ToCurrencyCode);
            existingRate.SetToCurrencyName(rate.ToCurrencyName);

            await this._dbContext.SaveChangesAsync();
        }
        
        /// <summary>
        /// Deletes an existing foreign exchange rate from the database
        /// </summary>
        /// <param name="rate">The rate to be deleted</param>
        public async Task DeleteAsync(ForeignExchangeRate rate)
        {
            this._dbContext.ForeignExchangeRates.Remove(rate);

            await this._dbContext.SaveChangesAsync();
        }
        
        /// <summary>
        /// Verifies if a foreign exchange rate exists in the database
        /// </summary>
        /// <param name="id">The id of the existing rate</param>
        /// <returns>true if it exists or false if it doesn't</returns>
        public async Task<bool> ExistsAsync(Guid id)
        {
            return await this._dbContext.ForeignExchangeRates.AnyAsync(x => x.Id == id);
        }
    }
}