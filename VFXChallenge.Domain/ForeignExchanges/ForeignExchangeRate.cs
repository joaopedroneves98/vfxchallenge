namespace VFXChallenge.Domain.ForeignExchanges
{
    using Abstractions;
    
    /// <summary>
    /// The Foreign Exchange Rate entity
    /// </summary>
    public class ForeignExchangeRate : Entity
    {
        public ForeignExchangeRate()
        {
        }
        
        public ForeignExchangeRate(string fromCurrencyCode, string toCurrencyCode, decimal bid, decimal ask, string fromCurrencyName, string toCurrencyName)
            : base(Guid.NewGuid())
        {
            this.FromCurrencyCode = fromCurrencyCode;
            this.ToCurrencyCode = toCurrencyCode;
            this.Bid = bid;
            this.Ask = ask;
            this.FromCurrencyName = fromCurrencyName;
            this.ToCurrencyName = toCurrencyName;
            this.Created = DateTime.UtcNow;
            this.Updated = DateTime.UtcNow;
        }
        
        /// <summary>
        /// The From Currency Code (ISO 4217)
        /// </summary>
        public string FromCurrencyCode { get; private set; }

        /// <summary>
        /// The To Currency Code (ISO 4217)
        /// </summary>
        public string ToCurrencyCode { get; private set; }
        
        /// <summary>
        /// The From Currency Name
        /// </summary>
        public string FromCurrencyName { get; private set; }

        /// <summary>
        /// The To Currency Name
        /// </summary>
        public string ToCurrencyName { get; private set; }

        // Decimal type was chosen, even though it is less performant, it's more precise. 
        // https://learn.microsoft.com/en-us/dotnet/csharp/language-reference/builtin-types/floating-point-numeric-types?redirectedfrom=MSDN#characteristics-of-the-floating-point-types
        
        /// <summary>
        /// The Bid price
        /// </summary>
        public decimal Bid { get; private set; }

        /// <summary>
        ///  The Ask price
        /// </summary>
        public decimal Ask { get; private set; }

        public void SetFromCurrencyCode(string fromCurrencyCode)
        {
            if (fromCurrencyCode is null)
            {
                throw new ArgumentNullException();
            }
            
            this.FromCurrencyCode = fromCurrencyCode;
            this.Updated = DateTime.UtcNow;
        }
        
        public void SetToCurrencyCode(string toCurrencyCode)
        {
            if (toCurrencyCode is null)
            {
                throw new ArgumentNullException();
            }
            
            this.ToCurrencyCode = toCurrencyCode;
            this.Updated = DateTime.UtcNow;
        }
        
        public void SetFromCurrencyName(string fromCurrencyName)
        {
            if (fromCurrencyName is null)
            {
                throw new ArgumentNullException();
            }
            
            this.FromCurrencyName = fromCurrencyName;
            this.Updated = DateTime.UtcNow;
        }
        
        public void SetToCurrencyName(string toCurrencyName)
        {
            if (toCurrencyName is null)
            {
                throw new ArgumentNullException();
            }
            
            this.ToCurrencyName = toCurrencyName;
            this.Updated = DateTime.UtcNow;
        }
        
        public void SetBid(decimal bid)
        {
            this.Bid = bid;
            this.Updated = DateTime.UtcNow;
        }
        
        public void SetAsk(decimal ask)
        {
            this.Ask = ask;
            this.Updated = DateTime.UtcNow;
        }
    }
}