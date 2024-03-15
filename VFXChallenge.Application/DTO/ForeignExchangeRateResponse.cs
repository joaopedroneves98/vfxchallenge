namespace VFXChallenge.Application.DTO
{
    /// <summary>
    /// The response DTO for the ForeignExchangeRate entity
    /// </summary>
    public class ForeignExchangeRateResponse
    {
        /// <summary>
        /// The Foreign Exchange Rate Id
        /// </summary>
        public Guid Id { get; set; }
        
        /// <summary>
        /// The From Currency Code (ISO 4217)
        /// </summary>
        public string FromCurrencyCode { get; set; }

        /// <summary>
        /// The To Currency Code (ISO 4217)
        /// </summary>
        public string ToCurrencyCode { get; set; }
        
        /// <summary>
        /// The From Currency Name
        /// </summary>
        public string FromCurrencyName { get; set; }

        /// <summary>
        /// The To Currency Name
        /// </summary>
        public string ToCurrencyName { get; set; }

        /// <summary>
        /// The Bid price
        /// </summary>
        public decimal Bid { get; set; }

        /// <summary>
        ///  The Ask price
        /// </summary>
        public decimal Ask { get; set; }
        
        /// <summary>
        /// The datetime when the entity was updated
        /// </summary>
        public DateTime Updated { get; set; }
        
        // Opted to leave the Created property out of the response DTO as it doesn't have much value to a consumer of the api
    }
}