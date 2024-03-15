namespace VFXChallenge.Application.DTO
{
    /// <summary>
    /// The Create DTO for the ForeignExchangeRate entity
    /// </summary>
    public class CreateForeignExchangeRate
    {
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
    }
}