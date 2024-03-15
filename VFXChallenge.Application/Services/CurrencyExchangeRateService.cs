namespace VFXChallenge.Application.Services
{
    using Abstractions;

    using Domain.ForeignExchanges;

    using DTO;

    using FluentValidation;

    using Infrastructure.AlphaVantageApi;
    using Infrastructure.Exceptions;
    using Infrastructure.Messaging;
    using Infrastructure.Repositories;

    using ValidationException = Infrastructure.Exceptions.ValidationException;

    /// <summary>
    /// This Service will orchestrate all the Application related operations (CRUD) for the Currency Exchange Rates 
    /// </summary>
    public class CurrencyExchangeRateService : ICurrencyExchangeRateService
    {
        private readonly IValidator<CreateForeignExchangeRate> _createForeignExchangeRateValidator;
        private readonly IValidator<UpdateForeignExchangeRate> _updateForeignExchangeRateValidator;
        
        private readonly IAlphaVantageGateway _alphaVantageGateway;

        private readonly IForeignExchangeRatesRepository _foreignExchangeRatesRepository;

        private readonly IProduceForeignExchangeRateCreatedMessage _produceForeignExchangeRateCreatedMessage;

        public CurrencyExchangeRateService(
            IAlphaVantageGateway alphaVantageGateway,
            IForeignExchangeRatesRepository foreignExchangeRatesRepository, 
            IProduceForeignExchangeRateCreatedMessage produceForeignExchangeRateCreatedMessage, 
            IValidator<CreateForeignExchangeRate> createForeignExchangeRateValidator, 
            IValidator<UpdateForeignExchangeRate> updateForeignExchangeRateValidator)
        {
            this._alphaVantageGateway = alphaVantageGateway;
            this._foreignExchangeRatesRepository = foreignExchangeRatesRepository;
            this._produceForeignExchangeRateCreatedMessage = produceForeignExchangeRateCreatedMessage;
            this._createForeignExchangeRateValidator = createForeignExchangeRateValidator;
            this._updateForeignExchangeRateValidator = updateForeignExchangeRateValidator;
        }

        /// <summary>
        /// Gets the currency pair from the database or from the API if it doesn't exist in the database
        /// </summary>
        /// <param name="fromCurrency">From Currency Code</param>
        /// <param name="toCurrency">To Currency Code</param>
        /// <returns>The pair exchange rate value</returns>
        /// <exception cref="Infrastructure.Exceptions.ValidationException"></exception>
        /// <exception cref="NotFoundException"></exception>
        public async Task<ForeignExchangeRateResponse> GetAsync(string fromCurrency, string toCurrency)
        {
            if (string.IsNullOrWhiteSpace(fromCurrency) || 
                string.IsNullOrWhiteSpace(toCurrency))
            {
                throw new ValidationException("From Currency and To Currency are required");
            }
            
            var response = await this._foreignExchangeRatesRepository.GetAsync(fromCurrency, toCurrency);
            if (response is null)
            {
                // check if data is available in the database, if not request the api
                response = await this._alphaVantageGateway.GetExchangeRateForCurrencyPairAsync(fromCurrency, toCurrency);

                if (response is null)
                {
                    throw new NotFoundException("Rate not found.");
                }

                // save the response in the database
                await this._foreignExchangeRatesRepository.CreateAsync(response);
                
                // When added to the database, fire event (kafka)
                await this._produceForeignExchangeRateCreatedMessage.ProduceAsync(response);
            }
            
            return new ForeignExchangeRateResponse
            {
                Id = response.Id,
                FromCurrencyCode = response.FromCurrencyCode,
                FromCurrencyName = response.FromCurrencyName,
                ToCurrencyCode = response.ToCurrencyCode,
                ToCurrencyName = response.ToCurrencyName,
                Ask = response.Ask,
                Bid = response.Bid,
                Updated = response.Updated
            };
        }

        /// <summary>
        /// Creates the exchange rate with the provided data
        /// </summary>
        /// <param name="request">Request with the currency pair data</param>
        /// <returns>The created exchange rate.</returns>
        /// <exception cref="ValidationException"></exception>
        /// <exception cref="ApplicationErrorException"></exception>
        public async Task<ForeignExchangeRateResponse> CreateAsync(CreateForeignExchangeRate request)
        {
            // Perform validations on request (fluent validations)
            var validationResult = this._createForeignExchangeRateValidator.Validate(request);
            if (!validationResult.IsValid)
            {
                throw new ValidationException(string.Join(";", validationResult.Errors)); // This should be improved with proper exceptions for validations
            }

            var existingExchangePair = await this._foreignExchangeRatesRepository.GetAsync(request.FromCurrencyCode, request.ToCurrencyCode);
            if (existingExchangePair != null)
            {
                throw new ApplicationErrorException("The provided Foreign Exchange Rate Pair already exists.");
            }

            // Use Automapper
            var rate = new ForeignExchangeRate(
                request.FromCurrencyCode,
                request.ToCurrencyCode,
                request.Bid,
                request.Ask,
                request.FromCurrencyName,
                request.ToCurrencyName);

            var response = await this._foreignExchangeRatesRepository.CreateAsync(rate);

            // When added to the database, fire event (kafka)
            await this._produceForeignExchangeRateCreatedMessage.ProduceAsync(response);
            
            return new ForeignExchangeRateResponse
            {
                Id = response.Id,
                FromCurrencyCode = response.FromCurrencyCode,
                FromCurrencyName = response.FromCurrencyName,
                ToCurrencyCode = response.ToCurrencyCode,
                ToCurrencyName = response.ToCurrencyName,
                Ask = response.Ask,
                Bid = response.Bid,
                Updated = response.Updated
            };
        }
        
        /// <summary>
        /// Fully updates an existing currency exchange rate pair
        /// </summary>
        /// <param name="request">The values to update the exchange rate pair with</param>
        /// <param name="id">The id of the exchange rate pair to be updated</param>
        /// <exception cref="NotFoundException"></exception>
        public async Task UpdateAsync(UpdateForeignExchangeRate request, Guid id)
        {
            // Perform validations on request (fluent validations)
            var validationResult = this._updateForeignExchangeRateValidator.Validate(request);
            if (!validationResult.IsValid)
            {
                throw new ValidationException(string.Join(";", validationResult.Errors)); // This should be improved with proper exceptions for validations
            }

            var rateExists = await this._foreignExchangeRatesRepository.ExistsAsync(id);
            if (!rateExists)
            {
                throw new NotFoundException("No rate found for the provided Id.");
            }
            
            var rate = new ForeignExchangeRate(
                request.FromCurrencyCode,
                request.ToCurrencyCode,
                request.Bid,
                request.Ask,
                request.FromCurrencyName,
                request.ToCurrencyName);

            await this._foreignExchangeRatesRepository.UpdateAsync(id, rate);
            
            // Fire event
            await this._produceForeignExchangeRateCreatedMessage.ProduceAsync(rate);
        }
        
        /// <summary>
        /// Deletes an existing currency exchange pair
        /// </summary>
        /// <param name="id">The Id of the pair to be deleted.</param>
        /// <exception cref="NotFoundException"></exception>
        public async Task DeleteAsync(Guid id)
        {
            var existingRate = await this._foreignExchangeRatesRepository.GetAsync(id);
            if (existingRate is null)
            {
                throw new NotFoundException("No rate found for the provided Id.");
            }

            await this._foreignExchangeRatesRepository.DeleteAsync(existingRate);
        }
    }
}