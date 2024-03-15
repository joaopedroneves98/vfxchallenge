namespace VFXChallenge.Api.Controllers
{

    using System.Net;

    using Application.Abstractions;
    using Application.DTO;

    using Microsoft.AspNetCore.Mvc;

    [Route("api/v1/[controller]")]
    [ApiController]
    public class ForeignExchangeRatesController : ControllerBase
    {
        private readonly ICurrencyExchangeRateService _currencyExchangeRateService;

        public ForeignExchangeRatesController(ICurrencyExchangeRateService currencyExchangeRateService)
        {
            this._currencyExchangeRateService = currencyExchangeRateService;
        }

        /// <summary>
        /// Gets an existing ForeignExchangeRate
        /// </summary>
        /// <param name="fromCurrency">The from currency code</param>
        /// <param name="toCurrency">The to currency code</param>
        /// <returns></returns>
        [HttpGet(Name = "GetAsync")]
        [ProducesResponseType(typeof(ForeignExchangeRateResponse), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        public async Task<IActionResult> GetAsync([FromQuery] string fromCurrency, [FromQuery] string toCurrency)
        {
            return this.Ok(await this._currencyExchangeRateService.GetAsync(fromCurrency, toCurrency));
        }

        /// <summary>
        /// Creates a new ForeignExchangeRate
        /// </summary>
        /// <param name="request">The ForeignExchangeRate to be created</param>
        /// <returns></returns>
        [HttpPost(Name = "PostAsync")]
        [ProducesResponseType(typeof(ForeignExchangeRateResponse), (int)HttpStatusCode.Created)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> PostAsync([FromBody] CreateForeignExchangeRate request)
        {
            var response = await this._currencyExchangeRateService.CreateAsync(request);

            return this.CreatedAtRoute("GetAsync", new
                {
                    fromCurrency = response.FromCurrencyCode,
                    toCurrency = response.ToCurrencyCode
                },
                response);
        }

        /// <summary>
        /// Updates an existing ForeignExchange Rate
        /// </summary>
        /// <param name="id">The id of the rate to be updated</param>
        /// <param name="value">The values to update the entity with</param>
        /// <returns></returns>
        [HttpPut("{id}", Name = "PutAsync")]
        [ProducesResponseType((int)HttpStatusCode.NoContent)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> PutAsync(Guid id, [FromBody] UpdateForeignExchangeRate value)
        {
            await this._currencyExchangeRateService.UpdateAsync(value, id);

            return this.NoContent();
        }

        /// <summary>
        /// Deletes a ForeignExchangeRate
        /// </summary>
        /// <param name="id">The id of the rate to be deleted</param>
        /// <returns></returns>
        [HttpDelete("{id}", Name = "DeleteAsync")]
        [ProducesResponseType((int)HttpStatusCode.NoContent)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        public async Task<IActionResult> DeleteAsync(Guid id)
        {
            await this._currencyExchangeRateService.DeleteAsync(id);

            return this.NoContent();
        }
    }
}